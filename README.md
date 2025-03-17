# Enemy AI with NavMesh in Unity

Este proyecto implementa un sistema de inteligencia artificial para un enemigo en un juego en 3D usando **Unity** y **NavMesh** para navegar por el terreno. El enemigo puede detectar al jugador, patrullar una zona, perseguirlo y atacarlo, así como huir cuando su salud es baja.

## Descripción del Proyecto

El enemigo tiene un comportamiento dinámico basado en su salud y la proximidad al jugador. Utiliza un sistema de navegación basado en **NavMesh** (superficie y agente) para interactuar con el entorno y ejecutar los siguientes estados:

- **Patrullando**: El enemigo se mueve aleatoriamente por la zona hasta encontrar al jugador.
- **Persiguiendo al jugador**: Si el jugador está dentro del alcance de visión del enemigo, el enemigo lo perseguirá.
- **Atacando al jugador**: Si el jugador está dentro del alcance de ataque, el enemigo intentará atacarlo.
- **Huyendo**: Si la salud del enemigo cae por debajo de un umbral, el enemigo intentará alejarse del jugador.

## Características

- **Detección de jugador**: Usa un rango de visión y un rango de ataque para determinar la proximidad del jugador.
- **Patrullaje aleatorio**: El enemigo puede patrullar por el mapa dentro de un rango específico.
- **Estados del enemigo**: El enemigo cambia de comportamiento según la salud y la proximidad al jugador.
- **Fugas**: Si la salud del enemigo es baja, este huye del jugador.

## Requisitos

- Unity 3D
- Conocimiento básico de **NavMesh** (superficie y agente)
- **TextMeshPro** para mostrar la salud del enemigo en pantalla.

## Instalación

1. Clona este repositorio a tu máquina local:

    ```bash
    git clone https://github.com/tu_usuario/enemy-ai-navmesh.git
    ```

2. Abre el proyecto en Unity.
3. Asegúrate de que tu escena tenga un **NavMesh** configurado correctamente.
4. Añade el script `EnemyAi` a un GameObject (como un cubo o esfera) que representará al enemigo.
5. Asocia el **NavMeshAgent** al GameObject del enemigo.
6. Configura el **TextMeshPro** para mostrar la salud del enemigo en la interfaz de usuario.


