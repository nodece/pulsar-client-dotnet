﻿module Simple

// Learn more about F# at http://fsharp.org
open System
open Pulsar.Client.Api
open FSharp.Control.Tasks.V2.ContextInsensitive
open Pulsar.Client.Common
open System.Text

let runSimple () =

    let serviceUrl = "pulsar://my-pulsar-cluster:30002"
    let subscriptionName = "my-subscription"
    let topicName = sprintf "my-topic-%i" DateTime.Now.Ticks;

    let client =
        PulsarClientBuilder()
            .ServiceUrl(serviceUrl)
            .Build()

    task {

        let! producer =
            ProducerBuilder(client)
                .Topic(topicName)
                .CreateAsync()

        let! consumer =
            ConsumerBuilder(client)
                .Topic(topicName)
                .SubscriptionName(subscriptionName)
                .SubscriptionType(SubscriptionType.Failover)
                .SubscribeAsync()

        let! consumer2 =
            ConsumerBuilder(client)
                .Topic(topicName)
                .SubscriptionName(subscriptionName)
                .SubscriptionType(SubscriptionType.Failover)
                .SubscribeAsync()
        
        let! messageId = producer.SendAsync(Encoding.UTF8.GetBytes(sprintf "Sent from F# at '%A'" DateTime.Now))
        printfn "MessageId is: '%A'" messageId

        let! message = consumer.ReceiveAsync()
        printfn "Received: %A" (message.Data |> Encoding.UTF8.GetString)

        do! consumer.AcknowledgeAsync(message.MessageId)
    }
