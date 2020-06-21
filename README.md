# The Foundry

A DIY .NET core app for managing a local MTG card collection.

Stack:

- MongoDb

- ASP.NET Core WebApi


Using
C# API
    magicthegathering.io

Icons
    https://andrewgioia.github.io/Keyrune/


### Some data definitions

- **Metacard** : a unique MTG card name. Represents a unique card name, which can be printed in many sets/variations.

- **MtgSet** : an MTG release containing many cards.

- **MtgCard** : a specific version of a Metacard. It has a specific set Id reference.

- **CardConstruct** : a single printed MtgCard. It references a specific MtgCard Id. Represents a single physical MTG card. Can be added to a Deck.

- **Deck** : a physical Deck made up of CardConstructs.