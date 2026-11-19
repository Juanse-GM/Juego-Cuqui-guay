# My Lover Streamer(s) — Dating Sim

Juego narrativo single-player tipo dating simulator donde la vida del jugador ocurre entre dos espacios conectados por un **único estado de partida (GameState)**:

1. **El mundo físico** — escenarios 3D con interacción Point & Click (cámara fija, hotspots, personajes).
2. **El teléfono** — interfaz 2D con chats, red social, calendario, banco, mapa, tienda y galería (apps por construir).

Una acción en el mundo 3D puede desbloquear un mensaje en el teléfono; una decisión tomada en el teléfono puede preparar un encuentro en el mundo. Ambos son dos formas de presentar y operar sobre los mismos sistemas.

---

## Requisitos

| Elemento | Versión |
|---|---|
| Unity | **6000.6.3f1** (Unity 6) — verificar en `ProjectSettings/ProjectVersion.txt` |
| Plantilla | Universal 3D (URP) |
| Input | Unity Input System (ya activo: `activeInputHandler: 1`) |
| Lenguaje | C# |

No se requieren paquetes externos adicionales.

---

## Cómo abrir el proyecto

1. Abre **Unity Hub**.
2. **Open → Add project from disk** → selecciona la carpeta `My Lover streamer(s)`.
3. Abre el proyecto con la versión **6000.6.3f1**.
4. Espera a que termine de importar (primera vez puede tardar unos minutos).
5. Comprueba que no haya **errores rojos** en la consola (ventana inferior). Si los hay, pulsa **Ctrl+R** para refrescar.

---

## Controles

| Tecla / acción | Qué hace |
|---|---|
| **Click izquierdo** | Seleccionar hotspots y personajes en el mundo 3D |
| **Tab** | Abrir / cerrar el teléfono |
| **Escape** | Cerrar el teléfono (si está abierto) |
| **F5** | Guardar la partida (depuración) |
| **F9** | Cargar la partida guardada (depuración) |

---

## Estado actual: Vertical Slice

El primer hito (prototipo jugable) está **implementado en código** y compila sin errores. Contiene:

- [x] Escena `Bootstrap` con servicios persistentes (GameRoot)
- [x] Escena `Location_Bedroom`: habitación 3D con cámara fija
- [x] 5 hotspots con hover (tint + tooltip), uno bloqueado por condición
- [x] Sistema de interacción: input → raycast → validación → dispatcher → efectos
- [x] Personaje placeholder (Ava) con diálogo ramificado (2 opciones)
- [x] Una elección modifica flag + afinidad de relación
- [x] Teléfono 2D: Home con badge de mensajes + pantalla de Chat
- [x] GameState compartido: día, bloque horario, dinero, flags, relaciones
- [x] Guardado / carga local en JSON con `schemaVersion`
- [x] Contenido de ejemplo en `Assets/_Project/Data/ContentDatabase.asset`

**Pendiente de configuración en el Editor** (guía en [`Docs/SETUP.md`](SETUP.md)):
las dos escenas, la capa `Interactable` y todas las referencias del Inspector.

**Fuera de alcance en esta etapa** (según el documento de arquitectura): movimiento libre, mundo abierto, combate, multijugador, redes sociales reales, chat real, IA narrativa, backend/suscripciones, los 45 días completos y el resto de personajes.

---

## Estructura del proyecto

```
My Lover streamer(s)/
├── Assets/
│   ├── _Project/                 ← TODO el contenido del juego
│   │   ├── Scenes/               ← Bootstrap, Location_Bedroom
│   │   ├── Scripts/
│   │   │   ├── Domain/           ← reglas del juego (sin UnityEngine)
│   │   │   ├── Application/      ← servicios y orquestación
│   │   │   ├── Presentation/     ← mundo 3D y UI
│   │   │   ├── Infrastructure/   ← guardado JSON
│   │   │   └── Content/          ← ContentDatabase (ScriptableObject)
│   │   ├── Data/                 ← ContentDatabase.asset y datos
│   │   ├── Prefabs/  Art/  Settings/
│   │   └── ...
│   ├── Scenes/                   ← SampleScene (plantilla)
│   └── Settings/                 ← assets de URP (plantilla)
├── Docs/                         ← documentación (esta carpeta)
│   ├── README.md                 ← este archivo
│   ├── SETUP.md                  ← guía de configuración en el Editor
│   └── ARCHITECTURE.md           ← arquitectura y cómo ampliar contenido
├── Packages/  ProjectSettings/
```

---

## Documentación

| Archivo | Qué contiene |
|---|---|
| [`Docs/SETUP.md`](SETUP.md) | Guía paso a paso (clic por clic) para montar las escenas y configurar todos los componentes en el Inspector |
| [`Docs/ARCHITECTURE.md`](ARCHITECTURE.md) | Arquitectura por capas, flujo de una interacción, tabla de scripts y recetas para añadir contenido nuevo |

Referencia técnica y de diseño principal: **`Technical_Architecture_Dating_Sim_v2.docx`** (documento externo, en tu carpeta de Descargas).
