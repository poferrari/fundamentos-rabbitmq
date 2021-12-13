namespace EstudoRabbitMQ.Services
{
    public static class PrefixMessageBrokerConst
    {
        public const string ExampleDirect = "example-direct";
        public const string ExampleFanout = "example-fanout";
        public const string ExampleTopic = "example-topic";

        public const string FileEvent = "file-event";
        public const string FileEventUnrouted = "file-event-unrouted";
        public const string FileEventDeadLetter = "file-event-dead-letter";
        public const string FileZip = "file-zip";
        public const string FileXml = "file-xml";
        public const string FileUnmapped = "file-unmapped";
        public const string FileEventManual = "file-event-manual";
    }
}
