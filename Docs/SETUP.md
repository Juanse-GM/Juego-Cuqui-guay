# Guía de configuración — Vertical Slice

## ¡Hola! Vamos a montar tu juego juntos

Tranquilo: este documento está pensado para que puedas seguirlo **aunque nunca hayas montado una escena en Unity**. Cada paso está explicado con mucho detalle: qué botón pulsar, dónde mirar, qué escribir y cómo comprobar que has hecho las cosas bien.

Si algo sale mal, **no pasa nada**. Nada de lo que hagas aquí es irreversible, y al final de la guía tienes una sección de problemas típicos con sus soluciones.

**Lo que ya está hecho por ti** (no tienes que tocarlo):

- Los 34 scripts del juego, ya escritos y compilando sin errores.
- El archivo de contenido `ContentDatabase.asset` con todos los datos de ejemplo (interacciones, diálogo, mensajes).
- La configuración de Input System y URP.

**Lo que vamos a hacer tú y yo ahora:**

1. Preparar una capa especial llamada `Interactable`.
2. Crear la escena `Bootstrap` (la escena de arranque con toda la interfaz).
3. Crear la escena `Location_Bedroom` (la habitación 3D con los hotspots).
4. Decirle a Unity qué escenas debe cargar al dar a Play.
5. Probar que todo funciona, con una lista de comprobación.

¿Listo? Empezamos.

---

## Antes de empezar: 6 palabras que necesitas saber

No hace falta que memorices nada; solo reconoce estas palabras cuando aparezcan:

1. **GameObject** — un objeto dentro de la escena (una cámarara, un cubo, un botón...). Es como una carpeta vacía a la que luego le añades cosas.
2. **Componente** — una pieza de comportamiento que se pega a un GameObject (por ejemplo, el que hace que reaccione al click). En Unity todo funciona así: GameObject + Componentes.
3. **Inspector** — la columna de la derecha de la pantalla. Cuando seleccionas algo, el Inspector muestra sus detalles y sus campos para editarlos.
4. **Jerarquía** — la lista de la izquierda con todos los objetos de la escena, organizados en árbol (padres e hijos).
5. **Escena** — una pantalla del juego (por ejemplo, una habitación). Los juegos se hacen escena a escena.
6. **Play** — el botón con forma de triángulo ▶ de arriba. Es "dar a jugar". Lo que hagas al jugar **no se guarda** en la escena: si cambias cosas estando en Play y luego paras, los cambios se pierden. **Configura siempre fuera de Play.**

---

## Paso 0 — Comprobar que todo está bien

Antes de tocar nada, vamos a asegurarnos de que el proyecto compila.

1. Abre Unity y espera a que termine de cargar (arriba puede aparecer una barrita de progreso; ten paciencia la primera vez).
2. Mira la **ventana de consola** (parte inferior de la pantalla). Si está minimizada: menú **Window → General → Console**.
3. **¿Ves textos rojos?** Entonces pulsa **Ctrl+R** (esto le dice a Unity que vuelva a revisar los scripts) y espera.
4. **¿La consola está limpia o solo tiene avisos amarillos?** ¡Perfecto! Todo está bien. Vamos al siguiente paso.

> **Desconfianza sana:** si ves algún error rojo que empiece por `error CS`, no continúes. Copia el mensaje y pásamelo; lo arreglamos antes de seguir.

---

## Paso 1 — Crear la capa `Interactable`

Una **capa** (Layer) es una etiqueta que le pones a los objetos para que Unity los agrupe. Nosotros necesitamos una etiqueta para decirle al juego: *"estos objetos son los que se pueden clickar"*.

1. En el menú superior de Unity, ve a **Edit → Project Settings...** (en Unity 6 puede estar en **Edit → Project Settings** o en el menú **Unity → Project Settings** si usas Mac).
2. Se abre una ventana con una lista de secciones a la izquierda. Busca y haz clic en **Tags and Layers**.
3. Verás una tabla llamada **Layers**. Tiene 32 casillas. Las 8 primeras están ocupadas por defecto (Default, TransparentFX, Ignore Raycast...). Las demás están vacías.
4. En la **casilla de usuario 8** (la primera vacía, cuenta desde 0; en pantalla verás filas numeradas), escribe exactamente:

   ```
   Interactable
   ```

   (Con la "I" mayúscula y el resto como está escrito. Los nombres de capa distinguen mayúsculas.)
5. Cierra la ventana de Project Settings. Ya está.

**Comprobación:** vuelve a **Edit → Project Settings → Tags and Layers** y verifica que en el usuario 8 pone `Interactable`. Si es así, perfecto: seguimos.

---

## Paso 2 — Crear la escena `Bootstrap`

Esta escena es el "cerebro" del juego: contiene los servicios que duran todas la partida (guardado, estado, contenido) y toda la interfaz (HUD, teléfono, diálogos).

### 2.1 Crear y guardar la escena

1. Menú **File → New Scene...**.
2. Elige la plantilla **Basic** (trae una cámara y una luz; nos vale) y haz clic en **Create**.
3. Verás una escena nueva e "Untitled". Guárdala ya mismo:
   - **File → Save As...**
   - Navega hasta `Assets/_Project/Scenes/`
   - Nómbrala exactamente: `Bootstrap`
   - **Save**

> **¿Por qué en esa carpeta?** Porque así todo lo nuestro vive junto en `Assets/_Project/`. No la guardes en `Assets/Scenes` (esa es de plantilla).

### 2.2 Crear el objeto `GameRoot` (el más importante)

1. En la **ventana Hierarchy** (lista de la izquierda), haz **clic derecho** → **Create Empty**. Se creará un objeto llamado "GameObject".
2. Con ese objeto seleccionado (está en azul), mira el **Inspector** (columna derecha):
   - En el campo del **nombre** (arriba del todo, donde pone "GameObject"), borra el texto y escribe: `GameRoot` (y pulsa Enter).
3. Añade el componente del juego:
   - En el Inspector, botón **Add Component** (abajo de todo).
   - Escribe `GameRoot` en el buscador y selecciónalo.
   - Aparece el componente con un campo: **Content**.
4. Asignar el contenido:
   - Arrastra el archivo `ContentDatabase` (está en `Assets/_Project/Data/ContentDatabase`) desde la ventana **Project** (parte inferior) hasta el campo **Content** del componente.
   - Si no ves el archivo: en Project, navega `Assets → _Project → Data`.
   - También puedes pulsar el botón circular a la derecha del campo y elegirlo de la lista.
5. Añade otro componente al mismo objeto `GameRoot`:
   - **Add Component → SaveHotkeys** (es el que nos dará F5/F9 para guardar y cargar).

**Comprobación:** selecciona `GameRoot`. En el Inspector debe verse:
- Componente **Game Root** con Content = `ContentDatabase`
- Componente **Save Hotkeys**

### 2.3 Crear la jerarquía de la interfaz

Ahora vamos a construir el árbol de objetos. **Lee primero el mapa completo** y luego créalo con calma, objeto por objeto. Los nombres importan (así los encuentras después).

```
GameRoot
├── EventSystem
├── UIRoot                    (Canvas)
│   ├── HUD
│   │   ├── DayLabel
│   │   ├── TimeLabel
│   │   └── MoneyLabel
│   ├── Tooltip
│   │   └── Tip
│   ├── Toast
│   │   └── Panel
│   ├── Dialogue
│   │   └── Panel
│   │       ├── Speaker
│   │       ├── Body
│   │       ├── Options
│   │       │   └── OptionButton
│   │       └── CloseButton
│   └── Phone
│       └── PhonePanel
│           ├── Home
│           │   ├── Title
│           │   ├── Badge
│           │   └── ChatButton
│           └── Chat
│               ├── BackButton
│               └── Messages
│                   └── MessageTemplate
```

Cómo se crea cada cosa:

- **GameObject vacío:** clic derecho en Hierarchy → **Create Empty**. Renómbralo seleccionándolo y cambiando el nombre en el Inspector (o con F2 en la Hierarchy).
- **Para hacerlo hijo de otro:** arrástralo encima del padre en la Hierarchy (o clic derecho → **Create Empty** estando el padre seleccionado).

Vamos por partes. No te apresures.

#### 2.3.1 EventSystem (los clics de los botones)

1. Clic derecho en Hierarchy → **UI → EventSystem** (si no lo ves en el menú, créalo con Create Empty y llámalo `EventSystem`).
2. Selecciona `EventSystem`:
   - En el Inspector verás el componente **Event System**.
   - Debajo probablemente haya un componente **Standalone Input Module**. **Elimínalo:** clic derecho sobre su título → **Remove Component**.
   - **Add Component → Input System UI Module**. (Si Unity te ofrece varios, elige el que se llame exactamente así.)
3. Arrastra `EventSystem` dentro de `GameRoot` en la Hierarchy (para que viva con él).

> **¿Por qué?** El módulo antiguo ("Standalone") no funciona con el nuevo sistema de input. Si no lo cambias, los botones no responderán a los clics. Es un error muy común; si lo olvidas, ya sabes dónde está.

#### 2.3.2 UIRoot (el Canvas)

1. Clic derecho en Hierarchy → **UI → Canvas**. Renómbralo a `UIRoot`.
2. Arrástralo dentro de `GameRoot`.
3. Selecciona `UIRoot` y revisa el Inspector:
   - Componente **Canvas**: Render Mode debe ser **Screen Space - Overlay** (es el que viene por defecto; si no, cámbialo).
   - Componente **Canvas Scaler**:
     - UI Scale Mode: **Scale With Screen Size**
     - Reference Resolution: **X = 1920**, **Y = 1080**
     - Match: **0.5** (deslizador a la mitad)
   - Componente **Graphic Raycaster**: debe estar (si no, **Add Component → Graphic Raycaster**). Sin él, los botones no reciben clics.

> **¿Para qué sirve esto?** El Canvas es donde vive toda la interfaz 2D (el teléfono, los textos...). El "Scaler" hace que tu interfaz se vea bien en pantallas de distinto tamaño — y ya estará preparada para móvil más adelante.

#### 2.3.3 HUD (día, hora y dinero)

1. Clic derecho sobre `UIRoot` → **UI → Text - TextMeshPro**. Si Unity te pregunta por **TMP Essential Resources**, pulsa **Import** (solo pasa la primera vez). Renómbralo a `DayLabel`.
2. Selecciona `DayLabel` y en el Inspector:
   - **Rect Transform**: en **Anchor Presets** (el cuadriculado de arriba), pulsa y mantén **Alt** (o Shift+Alt) y elige el cuadrante **superior izquierdo**. Así se ancla a la esquina superior izquierda.
   - Pos X: **20**, Pos Y: **−20** (el menos va delante: `-20`).
   - Text: `Día 1`
   - Font Size: **28**, Color: blanco.
3. Duplica el texto dos veces (selecciona `DayLabel` y pulsa **Ctrl+D** dos veces) y renómbralos:
   - `TimeLabel` → Pos Y: **−60**, Text: `Mañana`
   - `MoneyLabel` → Pos Y: **−100**, Text: `200 $`
4. Los tres deben quedar hijos directos de `UIRoot`. En el Inspector de cada uno, desmarca **Raycast Target** (está en el componente TextMeshPro - Text). Si no lo desmarcas, estos textos "atraparán" los clics y no llegarán a los hotspots.

#### 2.3.4 Tooltip (la etiqueta que sigue al ratón)

1. Clic derecho en `UIRoot` → **Create Empty** → renómbralo `Tooltip`.
   - **Importante:** este objeto debe quedar **activo** (tiene la casilla de la izquierda del nombre marcada).
2. Dentro de `Tooltip` (hazlo hijo), crea **UI → TextMeshPro** y renómbralo `Tip`.
   - En `Tip`, añade **Add Component → Image** para que tenga fondo (color oscuro, semitransparente, p. ej. negro con alpha 180).
   - Desmarca **Raycast Target** en el Image y en el Text.
   - Ajusta el tamaño: Width **220**, Height **44**.
3. Selecciona `Tooltip` → **Add Component → TooltipView**.
   - Campo **Tip Root**: arrastra `Tip`.
   - Campo **Label**: arrastra el objeto `Tip` (que tiene el TextMeshPro).

#### 2.3.5 Toast (los mensajes temporales)

1. Clic derecho en `UIRoot` → **Create Empty** → renómbralo `Toast` (**activo**).
2. Dentro de `Toast`, crea **UI → Image** → renómbralo `Panel`.
   - Debe empezar **INACTIVO** (desmarca la casilla de la izquierda del nombre).
   - Ancla: centro-abajo. Rect Transform: anclaje inferior central, Pos Y **80**, Width **900**, Height **70**.
   - Color: oscuro semitransparente.
3. Dentro de `Panel`, crea **UI → Text - TextMeshPro** → renómbralo `Text` (o déjalo como venga):
   - Ancla: estirado para ocupar todo el Panel (en Anchor Presets con Alt, el de "relleno completo").
   - Alineación: centrada (los tres puntitos centrados horizontalmente).
   - Font Size: **30**.
   - Desmarca **Raycast Target** (tanto en Image como en Text).
4. Selecciona `Toast` → **Add Component → ToastView**:
   - **Panel**: arrastra el objeto `Panel`
   - **Label**: arrastra el `Text` de dentro
   - **Duration**: `2.5`

#### 2.3.6 Dialogue (la ventana de conversación)

1. Clic derecho en `UIRoot` → **Create Empty** → renómbralo `Dialogue` (**activo**).
2. Dentro, crea **UI → Image** → renómbralo `Panel` → **INACTIVO**.
   - Rect Transform: ancla **abajo-estirado**: en Anchor Presets, con Alt pulsa el de abajo que estira en horizontal (o configúralo a mano: Anchor Min `(0, 0)`, Anchor Max `(1, 0)`, Pivot `(0.5, 0)`, Height **300**, Width 0).
   - Color: casi negro con algo de transparencia.
3. Dentro de `Panel`:
   - **UI → Text - TextMeshPro** → `Speaker`: arriba, font bold, tamaño ~32. Pos Y cerca del borde superior (p. ej. −20). Desmarca Raycast Target.
   - **UI → Text - TextMeshPro** → `Body`: debajo del Speaker, alto ~120, tamaño ~26. Desmarca Raycast Target.
   - **UI → Empty** (o Create Empty) → `Options`: debajo de Body. **Add Component → Vertical Layout Group** (con esto, los botones se apilan solos solos). Desmarca Raycast Target si aparece.
   - **UI → Button** → `OptionButton`: hazlo hijo de `Options`. **Déjalo INACTIVO** (nunca se verá tal cual: el juego lo clona). Ponle un TextMeshPro hijo con el texto `Opción de ejemplo` y un tamaño cómodo (Height **60** en su Rect Transform).
   - **UI → Button** → `CloseButton`: hijo de `Panel`, abajo a la derecha. Texto: `Cerrar`.
4. Selecciona `Dialogue` → **Add Component → DialoguePanel** y rellena:

   | Campo | Arrastra... |
   |---|---|
   | Panel Root | `Panel` |
   | Speaker Label | `Speaker` |
   | Body Label | `Body` |
   | Options Container | `Options` |
   | Option Template | `OptionButton` |
   | Close Button | `CloseButton` |

> Los botones de opción y "Cerrar" **no necesitan configurar nada en On Click**: el script se encarga de conectarlos al abrir el diálogo. Si ves un botón On Click vacío, déjalo vacío; está bien así.

#### 2.3.7 Phone (el teléfono)

1. Clic derecho en `UIRoot` → **Create Empty** → renómbralo `Phone` (**activo**).
2. Dentro de `Phone`, crea **UI → Image** → renómbralo `PhonePanel` → **INACTIVO**.
   - Rect Transform: ancla **centro**, Width **600**, Height **1000**, Pos X 0, Pos Y 0.
   - Color: gris oscuro o negro (será la "carcasa" del teléfono).
3. Dentro de `PhonePanel`:
   - **Create Empty** → `Home` (**activo**).
     - Dentro de `Home`:
       - **UI → Text - TextMeshPro** → `Title`: arriba, texto `Mensajes`, tamaño grande (40). Desmarca Raycast Target.
       - **UI → Image** → `Badge`: esquina superior derecha del título, tamaño 48×48, color rojo. Dentro, **TextMeshPro** → texto vacío (se rellenará solo), centrado.
       - **UI → Button** → `ChatButton`: centro, tamaño generoso (Width 400, Height 100), texto `Abrir chat`.
   - **Create Empty** → `Chat` → **INACTIVO**.
     - Dentro de `Chat`:
       - **UI → Button** → `BackButton`: arriba a la izquierda, texto `← Volver`.
       - **Create Empty** → `Messages`: ocupa el resto debajo. **Add Component → Vertical Layout Group** (y opcionalmente Content Size Fitter, Vertical Fit: Preferred Size — para que crezca hacia abajo).
       - Dentro de `Messages`: **UI → Text - TextMeshPro** → `MessageTemplate` → **INACTIVO**, ancho estirado (Width 500), alto ~50, desmarca Raycast Target.
4. Componentes:
   - Selecciona `PhonePanel` → **Add Component → AppNavigationManager**:
     - **Home Screen**: `Home`
     - **Chat Screen**: `Chat`
   - Selecciona `Phone` → **Add Component → SmartphoneManager**:
     - **Phone Panel**: `PhonePanel`
     - **Navigation**: arrastra el componente `AppNavigationManager` que está en `PhonePanel`
     - **Phone Button**: déjalo vacío (es opcional)
5. Conectar el botón "Volver":
   - Selecciona `BackButton`.
   - En el componente **Button**, busca **On Click ()** → pulsa **+**.
   - En el campo vacío de la izquierda, arrastra el objeto `PhonePanel`.
   - En el desplegable de la derecha elige: **AppNavigationManager → ShowHome**.

#### 2.3.8 Conectar HomeScreen y ChatScreen

1. Selecciona `Home` → **Add Component → HomeScreen**:
   - **Character Id**: `char_demo` (ya viene por defecto; si no, escríbelo)
   - **Unread Badge**: arrastra `Badge`
   - **Chat Button**: arrastra `ChatButton`
   - **Navigation**: arrastra el `AppNavigationManager` de `PhonePanel`
2. Selecciona `Chat` → **Add Component → ChatScreen**:
   - **Character Id**: `char_demo`
   - **Message Container**: arrastra `Messages`
   - **Message Template**: arrastra `MessageTemplate`

#### 2.3.9 Guardar

**File → Save Scene** (Ctrl+S). Con esto, el Bootstrap queda listo.

**Comprobación final del Paso 2:** despliega `UIRoot` en la Hierarchy y compara tu árbol con el mapa del apartado 2.3. Si algún nombre difiere, renómbralo (F2). Los nombres son importantes: los scripts buscan cosas por referencia en el Inspector, pero tenerlo ordenado te salvará más adelante.

---

## Paso 3 — Crear la escena `Location_Bedroom` (la habitación)

### 3.1 Crear y guardar la escena

1. **File → New Scene → Basic → Create**.
2. **File → Save As...** → carpeta `Assets/_Project/Scenes/` → nombre: `Location_Bedroom`.
3. Tendrás en la escena: `Main Camera`, `Directional Light` (y quizá un "Global Volume"; déjalo donde esté).

### 3.2 Colocar la cámara

1. Selecciona `Main Camera`:
   - Pos X `0`, Pos Y `3`, Pos Z `−7`
   - Rotation X `15`, Y `0`, Z `0`
2. Comprueba que **Tag** (arriba del Inspector) es **MainCamera** (viene así por defecto).

Con esto, la cámara mira hacia la habitación desde fuera, un poco hacia abajo.

### 3.3 Construir la habitación (con primitivas)

Clic derecho en Hierarchy → **3D Object → Cube** (o Plane). Para cada uno: créalo, renómbralo, y en el Inspector escribe la **Position** y **Scale** exactas:

| Objeto (renómbralo así) | Tipo | Position (X, Y, Z) | Scale (X, Y, Z) |
|---|---|---|---|
| `Floor` | Plane | `0, 0, 0` | `1, 1, 1` |
| `Wall_Back` | Cube | `0, 2.5, 5` | `10, 5, 0.3` |
| `Wall_Left` | Cube | `−5, 2.5, 0` | `0.3, 5, 10` |
| `Wall_Right` | Cube | `5, 2.5, 0` | `0.3, 5, 10` |
| `Bed` | Cube | `−3, 0.5, 3` | `2, 1, 3` |
| `Desk` | Cube | `3, 0.4, 4` | `2, 0.8, 1` |
| `Window` | Cube | `0, 3, 4.8` | `2.5, 1.5, 0.1` |
| `Drawer` | Cube | `3.5, 0.5, 2.2` | `0.8, 1, 0.8` |
| `Ava` | Capsule | `0, 1, 2` | `1, 1, 1` |

> **Truco para escribir los números:** en el campo Position/Scale puedes pegar directamente `0 2.5 5` (separado por espacios) en el primer campo y Unity lo reparte. O escribe uno a uno con calma.

**Mira la escena desde la pestaña Game** (pestaña junto a Scene, arriba). Si algo se ve raro, revisa las coordenadas. Puedes pulir la posición de la cámara después; lo importante es que veas la habitación.

### 3.4 Marcar los objetos interactivos

Estos 5 objetos serán los clicables: `Bed`, `Desk`, `Window`, `Drawer`, `Ava`.

Para **cada uno** de ellos:

1. Selecciona el objeto.
2. Arriba del Inspector, en el desplegable **Layer**, elige **Interactable** (la capa que creamos en el Paso 1).
   - Si Unity pregunta *"Do you want to set layer for child objects too?"* responde **No, Don't Change** (no tienen hijos).
3. Verifica que tiene un **Collider** (Cube y Capsule ya lo traen: BoxCollider / CapsuleCollider). Sin Collider, el click no puede detectarlo.
4. **Add Component → Hotspot** y rellena los campos **exactamente** (un error de una letra y la interacción no se encontrará):

| Objeto | hotspotId | interactionDefinitionId | display Name |
|---|---|---|---|
| `Bed` | `hotspot_bed` | `int_bed_inspect` | `Cama` |
| `Desk` | `hotspot_desk` | `int_desk_note` | `Escritorio` |
| `Window` | `hotspot_window` | `int_window_view` | `Ventana` |
| `Drawer` | `hotspot_drawer` | `int_drawer_locked` | `Cajón` |
| `Ava` | `hotspot_ava` | `int_talk_ava` | `Ava` |

> Si al añadir Hotspot Unity avisa *"This component requires a Collider"* → está bien, ya lo tiene; si te lo pide, añade **Add Component → Box Collider**.

### 3.5 Añadir los objetos de sistema

1. Clic derecho en Hierarchy → **Create Empty** → renómbralo `InteractionFeedback` → **Add Component → InteractionFeedback** (los campos por defecto están bien).
2. Clic derecho → **Create Empty** → renómbralo `Location` → **Add Component → LocationController**:
   - **Location Id**: `loc_bedroom` (ya viene por defecto).

### 3.6 Configurar WorldPointer en la cámara

1. Selecciona `Main Camera` → **Add Component → WorldPointer**.
2. Rellena:

   | Campo | Valor |
   |---|---|
   | View Camera | arrastra `Main Camera` (o déjalo vacío: se auto-busca) |
   | Interactable Mask | marca **solo** la casilla `Interactable` (desmarca Default) |
   | Feedback | arrastra el objeto `InteractionFeedback` |
   | Max Distance | `100` (por defecto) |
   | Tooltip Offset | `18, −18` (por defecto) |

> **El campo Interactable Mask es el más fácil de fallar:** si queda marcado `Default`, el juego intentará clicar también las paredes y el suelo. Asegúrate de que **solo** esté marcado `Interactable`.

### 3.7 Guardar

**Ctrl+S**. Escena lista.

---

## Paso 4 — Decirle a Unity qué escenas cargar

Si das a Play ahora mismo desde `Location_Bedroom`, verás un aviso en consola: *"GameRoot no encontrado"* — es normal: el cerebro vive en Bootstrap.

1. Abre la escena **Bootstrap** (doble clic en `Assets/_Project/Scenes/Bootstrap`).
2. Menú **File → Build Profiles...** (en versiones anteriores: **Build Settings**).
3. Busca la sección **Scene List** (o **Scenes In Build**).
4. Pulsa **Add Open Scenes** (asegúrate de que Bootstrap esté abierto) y repite con `Location_Bedroom`.
5. **El orden importa:** `Bootstrap` debe ser la **escena 0** (la primera de la lista). Si no lo es, arrástralo arriba.

**Comprobación:** en la lista debe verse:

```
0: Assets/_Project/Scenes/Bootstrap.unity
1: Assets/_Project/Scenes/Location_Bedroom.unity
```

---

## Paso 5 — ¡A probar! Lista de comprobación

Con `Bootstrap` abierta, pulsa **▶ Play**. Sigue la lista; cada fila tiene un resultado esperado. **No pasa nada si algo falla**: anota qué fila falló y mira la sección de problemas.

| # | Qué hacer | Qué deberías ver |
|---|---|---|
| 1 | Esperar a que cargue | La habitación 3D. Arriba a la izquierda: `Día 1`, `Mañana`, `200 $` |
| 2 | Pasar el ratón por Bed, Desk... | El objeto se ilumina en amarillo y aparece una etiqueta con su nombre (Cama, Escritorio...) |
| 3 | Click en **Cama** | Un mensaje abajo (toast): *"Una cama hecha con cuidado. Todavía no es hora de dormir."* Se puede repetir |
| 4 | Click en **Cajón** | Toast: *"No disponible ahora."* (está bloqueado a propósito) |
| 5 | Click en **Escritorio** y luego otra vez | 1ª vez: la nota. 2ª vez: *"Ya lo he hecho antes."* |
| 6 | Click en **Ventana** | Toast con la descripción **y** la hora cambia de `Mañana` a `Tarde` |
| 7 | Click en **Ava** | Se abre el diálogo abajo con dos botones de opción |
| 8 | Elegir *"Sí, encantado..."* | El diálogo pasa a la respuesta y cierra (o pulsa Cerrar). Internamente: flag `met_ava` + afinidad +10 |
| 9 | Pulsar **Tab** | Se abre el teléfono. El botón del chat muestra un badge con el número **2** |
| 10 | Click en "Abrir chat" | Ves 3 mensajes de Ava (los 2 nuevos aparecieron por tu decisión) |
| 11 | Pulsar **← Volver** y luego **Escape** | Vuelves a Home; Escape cierra el teléfono |
| 12 | Con el teléfono cerrado, click en la habitación | Los hotspots vuelven a reaccionar. **Durante el diálogo, en cambio, no reaccionan** (probando el paso 7 otra vez) |
| 13 | **F5** → parar Play ▶ → volver a dar Play | Todo restaurado: día, hora, dinero, afinidad, mensajes leídos |

**¡Enhorabuena!** Si las 13 filas pasan, tu vertical slice funciona de extremo a extremo: una acción en el mundo 3D alteró el estado del juego y el teléfono lo reflejó, y todo sobrevive a un guardado.

---

## Problemas típicos (y cómo arreglarlos)

Respira. Casi todo lo que puede salir mal tiene arreglo rápido.

| Si te pasa esto... | Es porque... | Haz esto: |
|---|---|---|
| Consola: *"GameRoot no encontrado"* | Estás en la escena Location sin pasar por Bootstrap | File → Open Scene → `Bootstrap`, y Play desde ahí |
| Los botones de UI no responden al click | El EventSystem tiene el módulo de input antiguo | En `EventSystem`: elimina *Standalone Input Module*, añade *Input System UI Module* |
| Al clicar hotspots "a través" del teléfono | El mundo no está bloqueando su input | Comprueba que `SmartphoneManager` está en `Phone` (**activo**) y que su campo *Phone Panel* apunta a `PhonePanel` |
| No aparece el tooltip ni el toast | Los padres están inactivos | `Tooltip` y `Toast` deben estar **activos**; solo `Tip` y `Panel` (los hijos) inactivos |
| El hotspot no reacciona en absoluto | Falta la capa, la máscara o el collider | Objeto con Layer `Interactable` + Collider + máscara en WorldPointer solo `Interactable` |
| El hover no cambia de color | El material no tiene `_BaseColor` | Usa materiales URP Lit (los de por defecto). O avísame y lo adaptamos a tu material |
| El chat no muestra los mensajes nuevos | El personaje o los IDs no coinciden | `characterId` = `char_demo` en HomeScreen **y** ChatScreen; los mensajes en ContentDatabase usan `char_demo` |
| *"Interacción no encontrada"* en consola (rojo) | Un `interactionDefinitionId` del hotspot tiene una letra de más | Compara carácter a carácter con la tabla del Paso 3.4 |
| El diálogo no abre | El `Content` de GameRoot no está asignado | GameRoot → campo *Content* → `ContentDatabase` |
| Unity muestra errores rojos `error CS` | Algo se movió en los scripts | Ctrl+R. Si persiste: copia el error y pásamelo |
| ¡Hice un lío en Play y se perdieron mis cambios! | Normal: en Play no se guarda | No es grave: los cambios de Play se descartan. Reconfigura fuera de Play |
| Los textos HUD "atrapan" los clicks | Raycast Target activado en textos | En cada TMP/Image no interactivo, desmarca **Raycast Target** |

---

## Mini glosario

| Término | Explicación simple |
|---|---|
| **GameObject** | Un objeto de la escena (cámara, cubo, botón...) |
| **Componente** | Un comportamiento pegado a un GameObject (un script, un Collider, un Image...) |
| **Inspector** | Panel derecho donde editas lo que tienes seleccionado |
| **Hierarchy** | Panel izquierdo con el árbol de objetos de la escena |
| **Scene / Escena** | Una pantalla del juego |
| **Play** | Ejecutar el juego en el Editor (los cambios de Play no se guardan) |
| **Layer / Capa** | Etiqueta agrupadora (nosotros usamos `Interactable`) |
| **Collider** | Forma invisible que permite que el rayo del ratón "toque" el objeto |
| **Canvas** | Contenedor de toda la interfaz 2D |
| **Raycast** | Un rayo invisible desde la cámara hasta donde haces click |
| **Hotspot** | Punto interactivo del mundo 3D |
| **Prefab** | Objeto guardado para reutilizarlo (aún no usamos ninguno) |
| **ScriptableObject** | Asset de datos (nuestro `ContentDatabase` es uno) |
| **Flag** | Variable booleana de memoria: "¿ocurrió esto?" (ej: `met_ava`) |
| **GameState** | El estado completo de la partida (día, dinero, flags, relaciones...) |
| **Dispatcher** | El central que recibe clicks y decide qué efectos ejecutar |

---

## ¿Y ahora qué?

Cuando el vertical slice funcione, el siguiente paso es ampliar **contenido** (nuevas interacciones, diálogos, apps del teléfono) sin tocar el motor del juego. La receta exacta para hacerlo está en [`ARCHITECTURE.md`](ARCHITECTURE.md), sección *"Cómo añadir contenido"*.

¡Tú puedes!
