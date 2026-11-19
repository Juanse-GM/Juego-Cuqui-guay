# Arquitectura del proyecto

Referencia técnica del vertical slice y guía para ampliar contenido sin romper el motor del juego. La referencia de diseño sigue siendo el documento **`Technical_Architecture_Dating_Sim_v2.docx`**; este archivo documenta lo que está implementado.

---

## 1. Principios

1. **Un solo GameState.** El mundo 3D y el teléfono son dos interfaces sobre los mismos datos. Ninguna pantalla guarda su propia copia de dinero, tiempo o relaciones.
2. **Las vistas no mandan.** UI y componentes 3D envían *intenciones* (IDs) a la capa de aplicación; ellos jamás modifican el estado directamente.
3. **El Domain no conoce Unity.** Las reglas del juego (condiciones, efectos, estado) viven en C# puro, sin `UnityEngine`, sin GameObjects ni escenas. Esto permite testearlo y moverlo de sitio.
4. **Hotspots y botones delgados.** Un hotspot solo dice *qué interacción* representa; una opción de diálogo solo dice *qué opciónId*. Las condiciones y efectos están en datos y los ejecutan sistemas reutilizables.
5. **Contenido en datos, estado en runtime.** Lo editable vive en `ContentDatabase` (ScriptableObject); lo que ocurre en la partida vive en `GameState` (serializable a JSON).
6. **Sin código arbitrario desde datos.** Las condiciones y efectos son listas cerradas de enums: el contenido no puede ejecutar C# libre.

---

## 2. Capas y dependencias

```
PRESENTATION  (mundo 3D + UI 2D)
  World:  Hotspot · WorldPointer · InteractionFeedback · LocationController
  UI:     DialoguePanel · SmartphoneManager · HomeScreen · ChatScreen
          HudView · TooltipView · ToastView · AppNavigationManager
        │  envían IDs / consultan servicios / se suscriben a eventos
        ▼
APPLICATION  (orquestación)
  GameRoot (composition root) · InteractionDispatcher · DialogueService
  ChatService · TimeManager · GameStateManager · GameEffectContext
        │  valida y ejecuta sobre el dominio
        ▼
DOMAIN  (reglas del juego — SIN UnityEngine)
  GameState · FlagStore · CharacterRuntimeState · TimeBlock
  ConditionEvaluator · EffectExecutor · IEffectContext
  InteractionDefinition · DialogueDefinition · ChatMessageDefinition
        ▲                          ▲
        │ define contenido          │ persiste estado
CONTENT (ScriptableObjects)    INFRASTRUCTURE
  ContentDatabase               SaveManager (JSON + schemaVersion)
                                SaveHotkeys (F5/F9)
```

**Reglas de dependencia:**

- Presentation → Application: permitido (los hotspots llaman al dispatcher).
- Application → Domain: permitido.
- Infrastructure → Domain: permitido (lee/escribe `GameState`).
- **Domain → cualquier otra cosa: PROHIBIDO.**
- Presentation no debe mutar `GameState` directamente; solo vía servicios.

---

## 3. Flujo de una interacción (el circuito central)

```
1. INPUT        WorldPointer detecta click (Mouse.current)
2. UI GUARD     ¿Teléfono abierto? ¿Diálogo activo? ¿Puntero sobre UI?
                → si cualquiera: se ignora el click
3. RAYCAST      Physics.Raycast con LayerMask "Interactable"
4. RESOLVE      hit.collider.GetComponentInParent<Hotspot>()
5. DELEGATE     Hotspot.TryInteract() → InteractionDispatcher.Dispatch(id)
6. VALIDATE     repetición (Once/Daily) + ConditionEvaluator
7. APPLY        EffectExecutor.Execute(effects, GameState, IEffectContext)
8. NOTIFY       GameStateManager.Notify() → OnStateChanged
9. PRESENT      las vistas se suscriben y re-renderizan (HUD, badge, chat...)
10. FEEDBACK    InteractionResult → ToastView (éxito con texto / motivo de fallo)
```

### Ejemplo end-to-end (mundo → teléfono)

```
Click en Ava
  → int_talk_ava: efecto StartDialogue(dlg_ava_meet)
  → DialogueService abre el nodo n_start
  → el jugador elige opt_hello
      · efecto SetFlag(met_ava)
      · efecto ChangeRelationship(char_demo, Affinity, +10)
  → GameStateManager.Notify()
  → HomeScreen recalcula: msg_ava_02 visible por FlagSet(met_ava)
                        msg_ava_03 visible por AffinityAtLeast(char_demo, 10)
  → el badge pasa a "2"
  → F5 guarda el JSON con esos flags y esa afinidad
```

---

## 4. Tabla de scripts (34)

### Domain — `Assets/_Project/Scripts/Domain/` (sin UnityEngine)

| Script | Responsabilidad |
|---|---|
| `Time/TimeBlock.cs` | Enum: Morning, Afternoon, Night, Dawn (el orden define el avance) |
| `Time/GameState.cs` | Estado completo de la partida + mutadores (AddMoney, AdvanceTimeBlock, GetOrCreateCharacter, AddItem...) |
| `Flags/FlagStore.cs` | Almacén de flags clave-valor serializable |
| `Relationships/CharacterRuntimeState.cs` | Afinidad/Confianza/Romance por personaje (0–100, con clamp) |
| `CharacterDefinition.cs` | Datos estáticos de personaje (id, nombre) |
| `Narrative/InteractionDefinition.cs` | Modelo de interacción + enums ConditionType y RepeatPolicy |
| `Narrative/EffectData.cs` | Modelo de efecto + enums EffectType y RelationshipStat |
| `Narrative/ConditionEvaluator.cs` | Evalúa condiciones contra GameState; devuelve motivo de fallo |
| `Narrative/EffectExecutor.cs` | Ejecuta efectos sobre GameState vía IEffectContext |
| `Narrative/DialogueDefinition.cs` | Grafo de diálogo: nodos y opciones con condiciones/efectos |
| `Narrative/ChatMessageDefinition.cs` | Mensaje de chat con condiciones de visibilidad |

### Application — `Scripts/Application/`

| Script | Responsabilidad |
|---|---|
| `GameRoot.cs` | Composition root: crea servicios, carga save, carga escena de localización, `BlocksWorldInteraction` |
| `GameStateManager.cs` | Dueño del GameState + evento `OnStateChanged` |
| `InteractionDispatcher.cs` | Valida y ejecuta interacciones (repetición + condiciones + efectos) |
| `DialogueService.cs` | Abrir/elegir/cerrar diálogos; evento `DialogueChanged` |
| `ChatService.cs` | Mensajes visibles, no leídos, marcar leídos |
| `TimeManager.cs` | Consume bloques horarios (día avanza al pasar de Dawn) |
| `GameEffectContext.cs` | Implementa IEffectContext: conecta efectos con tiempo y diálogos |

### Infrastructure — `Scripts/Infrastructure/Save/`

| Script | Responsabilidad |
|---|---|
| `SaveData.cs` | Envoltorio JSON: `schemaVersion` + `GameState` |
| `SaveManager.cs` | Guardar/cargar en `Application.persistentDataPath/save.json` |
| `SaveHotkeys.cs` | F5 guardar / F9 cargar con feedback por toast |

### Content — `Scripts/Content/`

| Script | Responsabilidad |
|---|---|
| `ContentDatabase.cs` | ScriptableObject con listas de interacciones, diálogos, mensajes y personajes; búsquedas por ID |

### Presentation/World — `Scripts/Presentation/World/`

| Script | Responsabilidad |
|---|---|
| `Hotspot.cs` | Delgado: hotspotId, interactionDefinitionId, displayName; delega al dispatcher |
| `WorldPointer.cs` | Input + guards + raycast + hover + click → feedback |
| `InteractionFeedback.cs` | Tint de hover vía MaterialPropertyBlock (`_BaseColor`) |
| `LocationController.cs` | Registra `locationId` en el GameState al cargar la escena |

### Presentation/UI — `Scripts/Presentation/UI/`

| Script | Responsabilidad |
|---|---|
| `DialoguePanel.cs` | Renderiza el nodo actual; clona botones de opción (sin lógica) |
| `HudView.cs` | Día / bloque horario / dinero (solo lectura) |
| `TooltipView.cs` | Etiqueta de nombre al hacer hover (single runtime `Current`) |
| `ToastView.cs` | Mensaje temporal (single runtime `Current`) |
| `Smartphone/SmartphoneManager.cs` | Abrir/cerrar teléfono (Tab/Escape) + bloquea el mundo |
| `Smartphone/AppNavigationManager.cs` | Alterna Home ↔ Chat |
| `Smartphone/HomeScreen.cs` | Badge de no leídos + acceso al chat |
| `Smartphone/ChatScreen.cs` | Lista mensajes visibles; marca leídos al mostrarse |

---

## 5. Referencia de datos (ContentDatabase)

Asset: **`Assets/_Project/Data/ContentDatabase.asset`**
Editar: seleccionarlo en Project → Inspector (o **Create → Dating Sim → Content Database** para crear otro).

### Enums (los verás como desplegables en el Inspector)

**ConditionType** (condiciones)

| Valor | Nombre | Parámetros |
|---|---|---|
| 0 | FlagSet | `key` = nombre del flag (debe ser true) |
| 1 | FlagNotSet | `key` = nombre del flag (debe ser false) |
| 2 | DayAtLeast | `value` = día mínimo |
| 3 | MoneyAtLeast | `value` = dinero mínimo |
| 4 | AffinityAtLeast | `key` = characterId, `value` = afinidad mínima |

**EffectType** (efectos)

| Valor | Nombre | Parámetros |
|---|---|---|
| 0 | SetFlag | `key` = nombre del flag |
| 1 | ClearFlag | `key` = nombre del flag |
| 2 | AddMoney | `value` = cantidad (acepta negativos; el saldo nunca baja de 0) |
| 3 | ChangeRelationship | `key` = characterId, `value` = delta (−100..100), `stat` = Affinity/Trust/Romance |
| 4 | ConsumeTime | `value` = nº de bloques horarios |
| 5 | StartDialogue | `key` = dialogueId |
| 6 | AddItem | `key` = itemId |

**RepeatPolicy** (en interacciones)

| Valor | Nombre | Comportamiento |
|---|---|---|
| 0 | Once | Una vez por partida (flag interno `done_<id>`) |
| 1 | Repeatable | Siempre (si las condiciones se cumplen) |
| 2 | Daily | Una vez por día (flag interno `daily_<id>_d<día>` |

### Contenido actual del vertical slice

| Tipo | IDs |
|---|---|
| Personaje | `char_demo` (Ava) |
| Interacciones | `int_bed_inspect` · `int_desk_note` (Once) · `int_window_view` (ConsumeTime 1) · `int_drawer_locked` (bloqueada: FlagSet `found_key`) · `int_talk_ava` (StartDialogue) |
| Diálogo | `dlg_ava_meet`: `n_start` → (`opt_hello` → `n_positive`) / (`opt_indifferent` → `n_negative`) |
| Mensajes | `msg_ava_01` (siempre) · `msg_ava_02` (FlagSet `met_ava`) · `msg_ava_03` (Affinity ≥ 10) |
| Hotspots | `hotspot_bed` · `hotspot_desk` · `hotspot_window` · `hotspot_drawer` · `hotspot_ava` |
| Localización | `loc_bedroom` |

### Flags en uso

| Flag | Quién lo pone | Para qué sirve |
|---|---|---|
| `note_read` | interacción `int_desk_note` | Marca que la nota fue leída |
| `found_key` | *(nadie aún)* | Desbloquearía el cajón; existe para demostrar un hotspot bloqueado |
| `met_ava` | opción `opt_hello` | Desbloquea `msg_ava_02` |
| `done_<id>` | dispatcher (Once) | Impide repetir interacciones únicas |
| `daily_<id>_d<n>` | dispatcher (Daily) | Limita a una vez por día |
| `read_<msgId>` | ChatService | Controla el badge de no leídos |

**Convención:** los flags guardados en la partida usan nombres en minúsculas con guiones bajos; nunca uses el texto visible ni nombres de GameObject como identificador.

---

## 6. Recetas: cómo añadir contenido

Todo esto se hace **sin escribir una línea de código**, en el Inspector del `ContentDatabase.asset`.

### 6.1 Añadir una interacción nueva

1. Selecciona `ContentDatabase` → lista **Interactions** → **+** (o incrementa Size).
2. Rellena:
   - `id`: p. ej. `int_mirror_look` (prefijo `int_`, minúsculas, guiones bajos)
   - `presentationText`: lo que dirá el toast al ejecutarla (puede ir vacío)
   - `repeatPolicy`: Once / Repeatable / Daily
   - `conditions`: qué debe cumplirse (puede quedar vacío = siempre disponible)
   - `effects`: qué pasa al ejecutarla
3. En la escena 3D: crea el objeto, **Layer = Interactable**, Collider, **Add Component → Hotspot**:
   - `hotspotId`: p. ej. `hotspot_mirror`
   - `interactionDefinitionId`: exactamente `int_mirror_look`
   - `displayName`: `Espejo`
4. Prueba en Play. Si la consola dice *"Interacción no encontrada"*, el ID no coincide carácter a carácter.

### 6.2 Añadir un diálogo nuevo

1. **Interactions** no participa: el diálogo se abre con un efecto `StartDialogue`.
2. En **Dialogues** → **+**:
   - `id`: p. ej. `dlg_barista_chat`
   - `startNodeId`: id del primer nodo
   - `nodes`: lista de nodos. Cada nodo:
     - `id`, `speakerId` (un `char_...`), `text`
     - `options`: cada opción con `id`, `text`, `nextNodeId` (vacío = cierra), `conditions` y `effects`
3. Para que algo lo abra: añade a una interacción (u opción) un efecto **StartDialogue** con `key = dlg_barista_chat`.
4. Reglas prácticas: prefiere ramas que convergen a nodos finales reutilizados; da IDs estables a cada opción (`opt_...`) para poder referenciarlos en flags o tests.

### 6.3 Añadir un mensaje de chat nuevo

1. En **Messages** → **+**:
   - `id`: p. ej. `msg_ava_04`
   - `characterId`: `char_demo` (o el personaje que corresponda)
   - `text`: el contenido
   - `visibility`: condiciones (vacío = siempre visible)
2. Aparecerá solo cuando las condiciones se cumplan, tanto en Home (badge) como en Chat.
3. El sistema de no leídos es automático: no hay que registrar nada más.

### 6.4 Añadir un personaje nuevo

En **Characters** → **+**: `id` (`char_luna`) y `displayName` (`Luna`). Para usarlo: `speakerId` en nodos, `characterId` en mensajes, `key` en efectos de relación, y `characterId` en HomeScreen/ChatScreen de su chat.

---

## 7. Sistema de guardado

- **Archivo:** `Application.persistentDataPath/save.json` (Windows: `%USERPROFILE%\AppData\LocalLow\<empresa>\<proyecto>\save.json`).
- **Formato:** JSON con envoltorio `SaveData { schemaVersion, gameState }`.
- **Versión actual:** `schemaVersion = 1`. Si cambia el modelo, incrementa la versión y añade migración en `SaveManager.LoadOrNull()`; por ahora, versiones distintas se ignoran con aviso (se empieza partida nueva en vez de corromper datos).
- **Qué se guarda:** día, bloque horario, dinero, localización, flags, estados de personaje, inventario. **No** se guardan assets, textos ni referencias a escenas (solo IDs).
- **Cuándo se guarda:** F5 manual (depuración). En producción: al cerrar el día, tras hitos y guardado manual (fase pendiente).
- **Al arrancar:** `GameRoot.Awake` carga el save si existe; si no, crea un `GameState` nuevo (Día 1, Mañana, 200 $).

---

## 8. Convención de IDs

| Tipo | Formato | Ejemplo |
|---|---|---|
| Personaje | `char_<nombre>` | `char_demo` |
| Localización | `loc_<nombre>` | `loc_bedroom` |
| Hotspot | `hotspot_<zona>_<cosa>` | `hotspot_desk` |
| Interacción | `int_<zona>_<acción>` | `int_desk_note` |
| Diálogo | `dlg_<personaje>_<tema>` | `dlg_ava_meet` |
| Nodo de diálogo | `n_<descripción>` | `n_start` |
| Opción | `opt_<descripción>` | `opt_hello` |
| Mensaje | `msg_<personaje>_<n>` | `msg_ava_01` |
| Evento futuro | `event_<día>_<tema>` | `event_day3_date` |

Nunca uses el texto traducido, el nombre del GameObject ni el display name como ID persistente: renombrar un objeto en Unity no debe romper una partida guardada.

---

## 9. Roadmap (según documento de arquitectura)

| Fase | Estado |
|---|---|
| 1. Fundación (URP, carpetas, Bootstrap, input) | Hecha |
| 2. Prototipo 3D (habitación, 5 hotspots, raycast, feedback) | Hecha (código; escenas a montar según SETUP.md) |
| 3. Personaje + diálogo (2 opciones, consecuencias) | Hecha |
| 4. Estado núcleo (tiempo, flags, relaciones, save/load) | Hecha |
| 5. Prototipo de teléfono (Home, Chat, navegación, bloqueo) | Hecha |
| 6. Vertical slice end-to-end | Hecha — pendiente de checklist manual (SETUP.md Paso 5) |
| 7. Sistemas de producción: EventScheduler, economía, inventario, calendario, posts, más localizaciones | Pendiente |
| 8. Producción de contenido: 4 romanceables, 5 secundarios, 45 días, finales | Pendiente |

**Al ampliar hacia la fase 7**, los puntos de extensión previstos por la arquitectura son:

- `ContentDatabase`: nuevas listas (EventDefinition, PostDefinition, LocationDefinition).
- Nuevo servicio `EventScheduler` en Application (disponibilidad por día/bloque + condiciones).
- Nuevas apps del teléfono reutilizando el patrón Home/Chat (AppNavigationManager ya es extensible).
- Guardado: ampliar `SaveData` con `completedEvents` y `decisionHistory` (incrementando `schemaVersion`).
