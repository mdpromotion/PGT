using UnityEngine;

namespace _Project.Features.ProceduralWorld.Domain
{
    [System.Serializable]
    public class NoiseSettings
    {
        [Header("Базовые настройки")]
        [Tooltip("Сид генератора шума. Одинаковый сид = одинаковый рельеф.")]
        public int Seed = 0;

        [Tooltip("Масштаб шума. Чем больше значение — тем более плавный и растянутый рельеф. Маленькие значения дают резкие, частые холмы.")]
        public float Scale = 50f;

        [Tooltip("Смещение шума в мировых координатах (доп. к worldOffset). Удобно для ручной подстройки узора.")]
        public Vector2 Offset = Vector2.zero;

        [Header("Октавы (детализация)")]
        [Tooltip("Количество слоёв шума. Больше октав — больше мелких деталей поверх основного рельефа.")]
        [Range(1, 8)] public int Octaves = 4;

        [Tooltip("Насколько сильно каждая следующая октава влияет на результат. Низкое значение = более гладкий рельеф, высокое = более шумный/резкий.")]
        [Range(0f, 1f)] public float Persistence = 0.5f;

        [Tooltip("Во сколько раз растёт частота с каждой октавой. Обычно 2. Больше — мельче и чаще детали от доп. октав.")]
        public float Lacunarity = 2f;

        [Header("Форма рельефа")]
        [Tooltip("Степень перераспределения высот (redistribution). >1 — делает низины более плоскими, а пики — более резкими и редкими (меньше гор). 1 — без изменений. <1 — сглаживает всё в холмы.")]
        [Range(0.1f, 5f)] public float RedistributionPower = 1.5f;

        [Tooltip("Кривая, дополнительно преобразующая итоговую высоту (0..1 -> 0..1). Позволяет вручную сделать больше равнин и меньше гор, задав плоский участок внизу кривой.")]
        public AnimationCurve HeightCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [Tooltip("Уровень 'моря' (0..1). Всё, что ниже этого значения, будет прижато к минимуму — создаёт плоские низины/океан вместо гор.")]
        [Range(0f, 1f)] public float SeaLevel = 0f;

        [Header("Итоговый масштаб высоты")]
        [Tooltip("Множитель финальной высоты террейна по Y.")]
        public float HeightMultiplier = 1f;
    }
}