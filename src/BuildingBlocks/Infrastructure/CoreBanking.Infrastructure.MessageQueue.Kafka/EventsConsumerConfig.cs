﻿namespace SuperSafeBank.Transport.Kafka
{
    public record EventsConsumerConfig
    {
        public EventsConsumerConfig(string kafkaConnectionString, string topicBaseName, string consumerGroup)
        {
            if (string.IsNullOrWhiteSpace(kafkaConnectionString))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(kafkaConnectionString));
            if (string.IsNullOrWhiteSpace(topicBaseName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(topicBaseName));
            if (string.IsNullOrWhiteSpace(consumerGroup))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(consumerGroup));
          
            KafkaConnectionString = kafkaConnectionString;
            TopicName = topicBaseName;
            ConsumerGroup = consumerGroup;
        }

        public string KafkaConnectionString { get; }
        public string TopicName { get; }
        public string ConsumerGroup { get; }
    }
}