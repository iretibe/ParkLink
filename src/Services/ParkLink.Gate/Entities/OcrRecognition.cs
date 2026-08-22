namespace ParkLink.Gate.Entities
{
    public sealed class OcrRecognition
    {
        private OcrRecognition() { }

        public Guid Id { get; private set; }
        public Guid GateId { get; private set; }
        public Guid DeviceId { get; private set; }
        public string LicensePlate { get; private set; } = null!;
        public decimal Confidence { get; private set; }
        public string? ImageReference { get; private set; }
        public DateTime RecognizedAtUtc { get; private set; }

        public static OcrRecognition Create(Guid gateId, Guid deviceId,
            string licensePlate, decimal confidence, string? imageReference = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(licensePlate, nameof(licensePlate));

            if (confidence < 0 || confidence > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(confidence), 
                    "OCR confidence cannot be between 0 and 1.");
            }

            return new OcrRecognition
            {
                Id = Guid.NewGuid(),
                GateId = gateId,
                DeviceId = deviceId,
                LicensePlate = licensePlate.Trim().ToUpperInvariant(),
                Confidence = confidence,
                ImageReference = imageReference,
                RecognizedAtUtc = DateTime.UtcNow
            };
        }
    }
}
