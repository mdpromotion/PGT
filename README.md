# PGT (Procedural-Generation Test)

[English Version](#english-version) | [Русская версия](#русская-версия)

---

<a name="english-version"></a>

# 🇬🇧 English Version

**PGT (Procedural-Generation Test)** is a technical prototype focused on procedural world generation and exploration systems in Unity. The project explores scalable approaches for building infinite worlds, including procedural landscapes, hydrology, runtime chunk streaming, and performance-oriented architecture designed for large-scale environments.

The main goal of PGT is experimenting with the technologies and systems required to create dynamic procedural worlds while maintaining stable runtime performance and clean, extensible code architecture.

## 🚀 Core Technologies & Approaches
- **Clean Architecture + MVP** — Modular architecture with clear separation between gameplay logic, infrastructure, and presentation.
- **Feature-first Structure** — Every system is isolated into independent features that can be extended without affecting unrelated parts.
- **SOLID Principles** — Designed for scalability, maintainability, and long-term project growth.
- **Data-Oriented Design** — Performance-critical systems built around Unity Jobs, Burst Compiler, and native memory structures.

## 🧠 Architecture & Patterns
- **Dependency Injection (VContainer)** — Lightweight dependency management and feature composition.
- **MVP (Model-View-Presenter)** — Separation between runtime presentation and application logic.
- **Factory Pattern** — Creation and lifecycle management of generated world objects.
- **Repository Pattern** — Runtime storage and lookup of generated world data.
- **Scheduler Pattern** — Controlled execution pipeline for asynchronous generation tasks.

## ⚙️ Tech Stack
- **Unity Jobs System** — Multithreaded world generation and background processing.
- **Unity Burst Compiler** — SIMD-optimized procedural calculations.
- **Unity Mathematics** — High-performance mathematical operations.
- **Native Collections** — Low-allocation runtime data processing.
- **ScriptableObjects** — Data-driven configuration of world, generation, and gameplay systems.

## 🧪 Performance & Optimization
- **Chunk Streaming** — Dynamic loading and unloading of world regions around the player.
- **Procedural Landscape Generation** — Burst-accelerated generation of terrain data.
- **Generation Cancellation** — Unnecessary background generation tasks are cancelled when no longer needed.
- **Memory Management** — Controlled lifetime of native resources and minimized runtime allocations.
- **Unity Profiler** — Continuous profiling and optimization of critical systems.
- **Low-GC Runtime** — Designed to reduce managed allocations during gameplay.

---

## 🏗 Project Structure (Feature-first)

`📦 Assets/_Project/Features/FeatureName`

- `Application` — Use cases and feature-specific logic
- `Domain` — Core rules, models, and independent systems
- `Infrastructure` — Unity-specific implementations and external integrations
- `Installer` — Dependency Injection registrations
- `Presentation` — Runtime views and MonoBehaviours
- `Data` — Configurations and ScriptableObject assets

## 🌎 World Systems

- **Infinite Procedural World** — Dynamic landscape generation and streaming around the player.
- **Procedural Hydrology System** — Generated rivers with terrain carving, natural flow paths, and water surface data.
- **Chunk-Based Water Rendering** — Runtime water visualization using generated masks, height data, and custom shaders.
- **Landscape Generation Pipeline** — Multithreaded world generation using Unity Jobs + Burst Compiler.
- **World Streaming System** — Automatic loading, unloading, and prioritization of nearby regions.
- **Environmental Interaction** — Player movement, sprinting, swimming, diving, and exploration mechanics.
- **Sound System** — Data-driven audio playback with configurable sound definitions and pooled audio sources.
- **Performance-Oriented Architecture** — Systems designed around scalability, profiling, and predictable runtime behavior.

## ✨ Current Features

- **Infinite Procedural World** — Large-scale procedural landscape generation around the player.
- **Procedural Rivers** — Natural river generation with terrain carving and improved water continuity.
- **Water System** — Runtime generated water surfaces with custom rendering support.
- **Chunk Streaming** — Dynamic world loading and unloading based on player location.
- **Chunk Scheduler** — Background generation pipeline with prioritization and cancellation support.
- **Landscape Continuity** — Seamless connection between generated world regions.
- **Player Exploration Systems** — Movement, sprinting, swimming, and diving.
- **Environmental Audio** — Sound feedback for player actions and world interaction.
- **Performance-Oriented Architecture** — Continuous optimization of generation speed, memory usage, and runtime stability.

## 🎯 Project Goals

- Explore modern approaches to procedural world generation.
- Build a scalable foundation for future survival / sandbox projects.
- Experiment with large-scale runtime world systems.
- Minimize CPU usage, GC allocations, and memory overhead.
- Develop reusable technologies for complex Unity environments.

---

<a name="русская-версия"></a>

# 🇷🇺 Русская версия

**PGT (Procedural-Generation Test)** — технический прототип, посвящённый процедурной генерации мира и созданию систем исследования в Unity. Проект исследует подходы к построению масштабируемых бесконечных миров: генерацию ландшафта, гидрологию, стриминг чанков и производительную архитектуру для больших процедурных окружений.

Главная цель PGT — экспериментирование с технологиями, необходимыми для создания динамических процедурных миров, сохраняя высокую производительность и чистую расширяемую структуру проекта.

## 🚀 Основные технологии и подходы

- **Clean Architecture + MVP** — разделение ответственности между логикой приложения, инфраструктурой и представлением.
- **Feature-first структура** — каждая система изолирована в отдельную фичу и может развиваться независимо.
- **SOLID принципы** — архитектура рассчитана на масштабирование и долгосрочное развитие.
- **Data-Oriented подход** — производительные системы построены вокруг Unity Jobs, Burst Compiler и Native Collections.

## 🧠 Архитектура и паттерны

- **Dependency Injection (VContainer)** — управление зависимостями и сборка игровых систем.
- **MVP (Model-View-Presenter)** — разделение представления и логики приложения.
- **Factory** — создание и управление жизненным циклом объектов процедурного мира.
- **Repository** — хранение и поиск данных сгенерированных областей.
- **Scheduler** — управление очередью асинхронной генерации и выполнением задач.

## ⚙️ Технологии

- **Unity Jobs System** — многопоточная генерация мира и фоновые вычисления.
- **Unity Burst Compiler** — оптимизированные процедурные вычисления.
- **Unity Mathematics** — высокопроизводительная математическая библиотека.
- **Native Collections** — эффективная работа с памятью без лишних аллокаций.
- **ScriptableObjects** — data-driven настройка параметров мира и игровых систем.

## 🧪 Оптимизация

- **Стриминг чанков** — динамическая загрузка и выгрузка мира вокруг игрока.
- **Процедурная генерация ландшафта** — генерация данных мира с использованием Burst и Jobs.
- **Отмена ненужной генерации** — прекращение вычислений для областей, которые больше не нужны.
- **Контроль памяти** — управление временем жизни Native ресурсов и снижение количества аллокаций.
- **Unity Profiler** — постоянный анализ и оптимизация критических систем.
- **Минимизация GC** — снижение количества managed-аллокаций во время выполнения.

---

## 🏗 Структура проекта (Feature-first)

`📦 Assets/_Project/Features/FeatureName`

- `Application` — сценарии использования и логика фичи
- `Domain` — основные правила, модели и независимые системы
- `Infrastructure` — Unity-реализации и внешние зависимости
- `Installer` — регистрация зависимостей
- `Presentation` — слой представления и MonoBehaviour
- `Data` — конфигурации и ScriptableObject-ресурсы

## 🌎 Системы мира

- **Бесконечный процедурный мир** — динамическая генерация и стриминг ландшафта вокруг игрока.
- **Гидрологическая система** — процедурные реки с изменением рельефа, естественными маршрутами течения и генерацией воды.
- **Система воды** — визуализация воды в чанках через процедурные данные и собственный шейдер.
- **Генерация ландшафта** — многопоточная генерация мира на Unity Jobs + Burst Compiler.
- **Стриминг мира** — автоматическая загрузка, выгрузка и приоритизация областей.
- **Взаимодействие с окружением** — движение, бег, плавание, погружение и исследование мира.
- **Звуковая система** — data-driven воспроизведение звуков с пулом AudioSource.
- **Архитектура, ориентированная на производительность** — постоянная работа над масштабируемостью и стабильностью.

## ✨ Реализовано на данный момент

- **Бесконечный процедурный мир** с генерацией областей вокруг игрока.
- **Процедурные реки** с естественной генерацией, изменением рельефа и улучшенной связностью.
- **Система воды** с процедурным созданием поверхности и визуализацией через шейдеры.
- **Стриминг чанков** с динамической загрузкой и выгрузкой мира.
- **Планировщик генерации** с очередью задач, приоритизацией и отменой ненужных вычислений.
- **Связность ландшафта** между процедурно созданными регионами.
- **Системы исследования мира** — движение, бег, плавание и погружение.
- **Звуковая система** с настройкой звуковых событий и управлением воспроизведением.
- **Производительная архитектура** с постоянной оптимизацией времени генерации и использования памяти.

## 🎯 Цели проекта

- Исследование современных подходов к процедурной генерации мира.
- Создание масштабируемой основы для будущего survival / sandbox проекта.
- Экспериментирование с системами больших процедурных окружений.
- Минимизация нагрузки на CPU, GC и использование памяти.
- Создание переиспользуемых технологий для сложных Unity-проектов.
