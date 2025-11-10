==========================================================
ARCADE + FIREBASE
Autor: Andres Eslava (Taller en clase)
==========================================================

Proyecto hecho en Unity para el taller de Conectividad con Firebase.
El objetivo es crear un minijuego tipo arcade donde caen objetos y el
jugador se mueve lateralmente para atraparlos o evitarlos, mientras se
registra puntaje, ranking (highscores) y analíticas personalizadas en
Firebase Firestore.

----------------------------------------------------------
FUNCIONAMIENTO GENERAL
----------------------------------------------------------

• Inicio: pantalla para escribir el nombre del jugador.
• Juego: el jugador se mueve a izquierda/derecha y toca objetos que caen.

“Good” (azul): otorga puntos extra y desaparece.

“Bad” (rojo): al tocarlo, termina la partida (Game Over).
• Puntaje: se actualiza en tiempo real en HUD.
• Fin: guarda puntaje en Firestore y muestra ranking Top-10.
Si el jugador no entra al Top-10, se muestra su puesto real al final.
• Analíticos: se registran estadísticas básicas por sesión.

Nota: Código en inglés, comentarios en español para explicar.
Estructura y sangrías limpias, nombres claros, escenas organizadas.

----------------------------------------------------------
FIREBASE (FIRESTORE)
----------------------------------------------------------

Se utiliza Firebase Firestore desde Unity con el archivo
google-services.json del proyecto:

Project ID: unityservicios25 (clave pública incluida en el JSON).

Inicialización única mediante FirebaseService (DontDestroyOnLoad).

Comprobación y reparación de dependencias al inicio.

Habilitación explícita de red (manejo de modo offline en editor).

En clase se puede abrir temporalmente read: if true para evidencias
(capturas), y luego volver a cerrar.

----------------------------------------------------------
COLECCIONES Y MODELO DE DATOS
----------------------------------------------------------

• highscores (ranking):

Estrategia “1 documento por jugador” → highscores/{playerName}

Campos: name (string), score (int), updatedAt (timestamp servidor)

Política: solo se sobrescribe si el nuevo score > score guardado.

• sessions (analíticos por partida):

Documento por sesión con: name (string), totalTime (float),
caught (int), dodged (int), attempts (int), endedAt (timestamp).

----------------------------------------------------------
ANALÍTICOS PERSONALIZADOS
----------------------------------------------------------

Se registran al terminar la partida:
• Tiempo total jugado (segundos).
• Objetos atrapados (caught) y esquivados (dodged).
• Número de intentos (persistido en PlayerPrefs).
• Fecha/hora de la sesión (timestamp de servidor).

Capturas de evidencia: consola de Firebase mostrando ambos sets de datos.

----------------------------------------------------------
INTERFAZ (UI)
----------------------------------------------------------

• Canvas con TextMeshPro (legible y coherente):

Start: InputField (TMP) para nombre + botón Play (deshabilitado si vacío).

Game: ScoreText (TMP) en HUD, feedback simple (popup “+X pts”).

Results: LastScoreText (TMP), RankingText (TMP) con Top-10.
• Popups de puntos: PopupText (TMP_Text + FloatingText) instanciado sobre
Popups (RectTransform full-screen).

----------------------------------------------------------
FLUJO DE ESCENAS
----------------------------------------------------------

Start → ingreso de nombre, validación, SceneManager.LoadScene("Game").

Game → loop simple de caída + control lateral, colisiones y fin.

Results → guarda en Firestore, lee Top-10 y muestra ranking ordenado.
Si el jugador no está en el Top-10, se añade su posición real al final.

Build Settings (orden):
0: Start | 1: Game | 2: Results

----------------------------------------------------------
ESTRUCTURA DE CARPETAS (Unity)
----------------------------------------------------------

Assets/
├─ Scripts/
│ ├─ Core/ (FirebaseService, ScoreRepository, AnalyticsRepository)
│ ├─ Game/ (GameManager, Spawner, PlayerController, FallingObject)
│ └─ UI/ (StartUI, ResultsUI, FloatingText)
├─ Scenes/ (Start.unity, Game.unity, Results.unity)
├─ Prefabs/ (Player, FallingGood, FallingBad, PopupText)
└─ Firebase/ (Plugins + dependencias oficiales)

Buena identación, nombres autoexplicativos, comentarios en español.

----------------------------------------------------------
CÓMO CORRER EL PROYECTO
----------------------------------------------------------

Clonar el repo e importar los paquetes de Firebase (o dejar los plugins
ya incluidos) y TextMeshPro.

Colocar google-services.json en Assets/.

Abrir la escena Start y dar Play.

Confirmar en consola de Firebase la aparición de:

Documento en highscores al finalizar una partida.

Documento en sessions con analíticos básicos.

Requisitos de colisión 2D (clave para el gameplay):

Player: Rigidbody2D = Kinematic, BoxCollider2D = IsTrigger = ON.

FallingGood/Bad: Rigidbody2D = Dynamic, BoxCollider2D = IsTrigger = OFF.

----------------------------------------------------------
EVIDENCIAS (ENTREGABLES)
----------------------------------------------------------

• Video (≤ 3 min) con flujo completo: Start → Game → Results (ranking).
• Capturas de Firestore:

highscores con el documento del jugador y orden de puntajes.

sessions (analíticos de la sesión).
• Enlaces:

Repositorio GitHub con proyecto organizado (.gitignore de Unity).

Enlace de video (YouTube o adjunto, según indicación del docente).

----------------------------------------------------------
NOTAS DE IMPLEMENTACIÓN
----------------------------------------------------------

• Ranking:

Se obtiene Top-10 desde Firestore (ordenado desc por score).

Si el jugador no está en Top-10, se calcula su posición real
(conteo de scores mayores) y se agrega resaltado al final.
• Timestamps:

Se usan FieldValue.ServerTimestamp para evitar depender del reloj local.
• Offline (Editor):

Se llama EnableNetworkAsync() al inicio para forzar online cuando sea
necesario. Si no hay internet, las escrituras pueden encolarse y enviarse
apenas vuelva la red.

----------------------------------------------------------
CONCLUSIÓN
----------------------------------------------------------

El proyecto cumple con:
• Mecánica arcade simple y funcional.
• Conexión a Firebase y persistencia en Firestore.
• Colección de “Highscores” y ranking visible en pantalla.
• Analíticos personalizados (tiempo, atrapados, esquivados, intentos).
• Flujo claro entre escenas y UI legible con TMP.
• Código en inglés con comentarios en español, buena organización y estilo.
