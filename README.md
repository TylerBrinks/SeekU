SeekU
=====

SeekU is a friendly way to get started with CQRS and Event Sourcing in .NET.

##What is it?
SeekU is a micro framework for plugging managing commands and persisting events.  SeekU takes care of the plumbing often associated
with CQRS and Event Sourcing.  It uses a convention-based approach for decoupling commands and events from the code that responds
to changes in your domain model.

##Is it easy?
Yep.  CQRS can be hard to get right.  At the most basic level, the out-of-the-box code will run in memory to demonstrate how simple 
it can be to build a domain model that builds a history of events.  There are also a bunch of "providers" to help you with sending
commands or persisting events for a more scalable app.

##Providers
A few of the providers included in the code base include:

- StructureMap and Ninject IoC providers
- SQL for storing events and snapshots in a SQL database
- MongoDB for storing events and snapshots in an MongoDB database
- File provider for storing events and snapshots on disk in JSON format
- Azure provider for storing events in Table storage, snapshots in blob storage, and sending commands and events via an Azure Service Bus

##Getting started
Try the Github Wiki.

##How about a sample?
The code base has 9 different samples showing how to use everything from NServiceBus to Azure to simple flat text files.  
Clone or download the project, set your start up sample project, and run.
