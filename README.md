# PGT (Procedural-Generation Test)

[English Version](#english-version) | [Русская версия](#русская-версия)

---

<a name="english-version"></a>

# 🇬🇧 English Version

**PGT (Procedural-Generation Test)** is a technical prototype focused on procedural terrain generation and world streaming in Unity. The project explores scalable architecture, high-performance terrain generation using Burst & Jobs, and efficient runtime streaming of an infinite world while maintaining stable performance and minimal memory allocations.

## 🚀 Core Technologies & Approaches
- **Clean Architecture + MVP** — Modular and maintainable codebase with clear separation of responsibilities.
- **Feature-first Structure** — Every feature is fully isolated and independently extensible.
- **SOLID Principles** — Designed for scalability and long-term maintenance.
- **Data-Oriented Design** — Performance-critical systems built around Unity's native collections and jobs.

## 🧠 Architecture & Patterns
- **Dependency Injection (VContainer)** — Lightweight dependency management.
- **MVP (Model-View-Presenter)** — Decoupled presentation and business logic.
- **Factory Pattern** — Terrain creation and lifecycle management.
- **Repository Pattern** — Runtime chunk storage and lookup.
- **Scheduler Pattern** — Controlled asynchronous chunk generation pipeline.

## ⚙️ Tech Stack
- **Unity Jobs System** — Multithreaded terrain generation.
- **Unity Burst Compiler** — SIMD-optimized procedural calculations.
- **Unity Mathematics** — High-performance math library.
- **Native Collections** — Zero-GC runtime memory management.
- **ScriptableObjects** — Data-driven biome and world configuration.

## 🧪 Performance & Optimization
- **Chunk Streaming** — Dynamic loading and unloading around the player.
- **Procedural Terrain Generation** — Burst-accelerated heightmap generation.
- **Generation Cancellation** — Chunks are cancelled when they leave the streaming area.
- **Memory Pooling & Native Memory Management** — Reduced allocations and predictable memory usage.
- **Unity Profiler** — Continuous profiling and optimization.
- **GC-Free Runtime** — Eliminating managed allocations during gameplay wherever possible.

---

## 🏗 Project Structure (Feature-first)

`📦 Assets/_Project/Features/FeatureName`

- `Application` — Use cases and application logic
- `Domain` — Core business rules and models
- `Infrastructure` — Unity-specific implementations
- `Installer` — Dependency Injection bindings
- `Presentation` — MonoBehaviours and presentation layer
- `Data` — Configurations and ScriptableObjects

## ✨ Current Features

- **Infinite Procedural World** — Dynamic terrain generation around the player.
- **Burst Terrain Generator** — Parallel heightmap generation using Unity Jobs + Burst.
- **Biome System** — Configurable biome-based terrain generation.
- **Chunk Streaming** — Automatic loading, unloading and prioritization of nearby chunks.
- **Chunk Scheduler** — Background generation queue with cancellation support.
- **Terrain Neighbor Stitching** — Automatic seamless connection between adjacent terrain chunks.
- **Performance-Oriented Architecture** — Designed with profiling and optimization as primary goals.

## 🎯 Project Goals

- Explore scalable procedural world generation techniques.
- Build a production-ready architecture for large Unity projects.
- Minimize CPU usage, GC allocations and runtime memory overhead.
- Serve as a foundation for future survival / sandbox projects.

---

<a name="русская-версия"></a>

# 🇷🇺 Русская версия

**PGT (Procedural-Generation Test)** — технический прототип, посвящённый процедурной генерации мира и исследованию производительной архитектуры в Unity. Проект разрабатывается как площадка для экспериментов с бесконечным миром, генерацией ландшафта, стримингом чанков и глубокой оптимизацией производительности.

## 🚀 Основные технологии и подходы

- **Clean Architecture + MVP** — чёткое разделение ответственности и независимость слоёв.
- **Feature-first структура** — каждая фича полностью изолирована и легко расширяется.
- **SOLID принципы** — архитектура, рассчитанная на долгосрочное развитие проекта.
- **Data-Oriented подход** — производительные системы построены на Unity Jobs и Native Collections.

## 🧠 Архитектура и паттерны

- **Dependency Injection (VContainer)** — управление зависимостями.
- **MVP (Model-View-Presenter)** — разделение представления и бизнес-логики.
- **Factory** — создание и управление жизненным циклом Terrain.
- **Repository** — хранение и поиск загруженных чанков.
- **Scheduler** — управление очередью генерации и выполнением Job.

## ⚙️ Технологии

- **Unity Jobs System** — многопоточная генерация чанков.
- **Unity Burst Compiler** — высокопроизводительные вычисления.
- **Unity Mathematics** — математическая библиотека для Burst.
- **Native Collections** — работа с памятью без лишнего GC.
- **ScriptableObjects** — data-driven настройка биомов и параметров мира.

## 🧪 Оптимизация

- **Стриминг чанков** — динамическая загрузка и выгрузка мира вокруг игрока.
- **Burst-генерация ландшафта** — вычисление карты высот в многопоточном режиме.
- **Отмена ненужной генерации** — чанки перестают генерироваться, если больше не нужны.
- **Контроль Native Memory** — управление временем жизни NativeArray и минимизация аллокаций.
- **Unity Profiler** — постоянный анализ производительности.
- **Минимизация GC** — устранение аллокаций во время игрового процесса.

---

## 🏗 Структура проекта (Feature-first)

`📦 Assets/_Project/Features/FeatureName`

- `Application` — сценарии использования и бизнес-логика
- `Domain` — модели и правила предметной области
- `Infrastructure` — реализации, зависящие от Unity
- `Installer` — регистрация зависимостей
- `Presentation` — слой представления (MonoBehaviour)
- `Data` — конфигурации и ScriptableObjects

## ✨ Реализовано на данный момент

- **Бесконечный процедурный мир** с динамической генерацией вокруг игрока.
- **Burst-генератор рельефа** на Unity Jobs + Burst Compiler.
- **Система биомов** с настраиваемыми параметрами генерации.
- **Стриминг чанков** с приоритизацией ближайших областей.
- **Планировщик генерации** с очередью, многопоточностью и отменой ненужных задач.
- **Автоматическое соединение Terrain** между соседними чанками.
- **Архитектура, ориентированная на производительность** с постоянной работой над снижением времени кадра и аллокаций памяти.

## 🎯 Цели проекта

- Исследование современных подходов к процедурной генерации мира.
- Построение масштабируемой архитектуры для крупных Unity-проектов.
- Минимизация нагрузки на CPU, GC и использование памяти.
- Создание технологической базы для будущего survival/sandbox проекта.
