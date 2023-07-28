# ddd-cargo-csharp
Cargo System built with DDD pattern. Summary and code from blue book [Domain Driven Design by Eric Evans](https://a.co/d/fRmp7M7)

## Table of Contents
<!--TOC-->
  - [Introduction](#introduction)
  - [Isolating the Domain: Introducing the Application](#isolating-the-domain-introducing-the-application)
  - [Distinguishing Entities and Value Objects](#distinguishing-entities-and-value-objects)
  - [Designing Associations in the Shipping Domain](#designing-associations-in-the-shipping-domain)
  - [Aggregate Boundaries](#aggregate-boundaries)
  - [Selecting Repositories](#selecting-repositories)
  - [Walking Through Scenarios](#walking-through-scenarios)
  - [Object Creation](#object-creation)
  - [Pause for Refactoring: An Alternative Design of the Cargo Aggregate](#pause-for-refactoring-an-alternative-design-of-the-cargo-aggregate)
  - [Modules in the Shipping Model](#modules-in-the-shipping-model)
<!--/TOC-->

## Introduction
We are developing new software for a cargo shipping company. The initial requirements are 3 basic functions
1. Track key handling of customer cargo
2. Book cargo in advance
3. Send invoices to customers automatically when the cargo reaches some point in its handling

![Class Diagram representing a Model of the Shipping Domain](docs/diagrams/DDD%20-%20Cargo%20-%201.%20Class%20Diagram%20representing%20a%20Model%20of%20the%20Shipping%20Domain.jpg?raw=true "Class Diagram representing a Model of the Shipping Domain")

[Fig 1. Class Diagram representing a Model of the Shipping Domain](https://miro.com/app/board/uXjVM5Gp1iE=/?moveToWidget=3458764558537298265&cot=14)

This model organizes domain knowledge and provides a language for the team. We can make statements like this:
1. "Multiple **Customers** are involved with a **Cargo**, each playing a different role."
2. "The **Cargo** delivery _goal_ is _specified_."
3. "A series of **Carrier Movements** satisfying the **Specification** will fulfill the delivery _goal_"

A **Handling Event** is a discrete action taken with the Cargo, such as 
1. loading it onto a ship
2. clearing it through customs

**Delivery Specification** defines a delivery goal, which at minimum would include a destination and an arrival time, but it can be more complex.

A _role_ distinguishes the different parts played by **Customers** in a shipment. 
1. One is the "shipper"
2. One the "receiver
3. One the "payer" and so on.

**Carrier Movement** represents one particular trip by a particular **Carrier** (such as a truck or a ship) from one **Location** to another. Cargoes can ride from place to place by being loaded onto **Carriers** for the duration of one or more **Carrier Movements**

**Delivery History** reflects what has actually happened to a **Cargo**, as opposed to the **Delivery Specification**, which describes goals. A **Delivery History** object can compute the current **Location** of the **Cargo** by analyzing the last load or unload and the destination of the corresponding **Carrier Movement**. A successful delivery would end with a **Delivery History** that satisfies the goals of the **Delivery Specification**.

In order to frame up a solid implementation, this model still needs some clarification and tightening. 

Remember, ordinarily, model refinement, design and implementation should go hand-in-hand in a iterative development process. But in this chapter we 
are starting with a relatively mature model, and changes will be motivated strictly by the need to connect
model with a practical implementation , employing the building block patterns.

## Isolating the Domain: Introducing the Application

To prevent domain responsibilities from being mixed with those of other parts of the system, let's apply layered architectur 
to mark off a domain layer.

 Without going into deep analysis, we can identify three user-level application function, which can assign to three application layer 
 classes.
 1. A **Tracking Query** that can access past and present handling of a particular **Cargo**
 2. A **Booking Application** that allows a new **Cargo** to be registered and prepares the system for it.
 3. An **Incident Logging Application** that can record each handling of the **Cargo** (providing the information that is found
 by the **Tracking Query**)

 These application classes are coordinators. They should not work out the answers to the questions they ask. That is 
 domain layer's job.

 ## Distinguishing Entities and Value Objects
 
 **Customer**  
 Let's start with an easy one. A Customer object represents a person or a company, an entity in the usual sense of the 
 word. The **Customer** object clearly has identity that matters to the user, so it is an **Entity** in the model. How
 to track it? Tax Id might be appropriate in some cases, but an international company could not use that. This question
 calls for consultation with a domain expert. We discovered that the company already has a customer database in which each
 Customer is assigned an Id number at first sales contact. This Id is already used throughout the company; using the number
 in our software establish continuity of identity between those systems.

 **Cargo**  
 Two identical crates must be distinguishable, so Cargo objects are **Entities**. In practice, all shipping companies assign
 tracking Ids to each piece of cargo. This Id will be automatically generated, visible to user, and in this case, probably
 conveyed to the customer at booking time.

 **Handling Event** and **Carrier Movement**
 We care about such individual incidents because they allow us to keep track of what is going on. They reflect real world 
 events, which are not usually interchangable, so they are **Entities**. Each **Carrier Movement** will be identified by a 
 code obtained from shipping schedule.  
 
 Another discussion with a domain expert reveals that **Handling Events** can be uniquely identified by the combination 
 of **Cargo** Id, completion time and type. For example, the same **Cargo** cannot be both loaded and unloaded at the 
 same time.

 **Location**  
 Two places with same name are not the same. Latitude and longitude could provide a unique key, but probably not a very
 practical one, since thos measurements are not of interest to most purposes of this system, and they would be fairly 
 complicated. More likely, the **Location** will be part of a geographical model of some kind that will relate places according
 to shipping lanes and other domain-specific concerns. So an arbitrary, internal, automatically generated identifier 
 will suffice.

 **Delivery History**  
 This is a tricky one. **Delivery Histories** are not interchangable, so they are Entities. But a  **Delivery History**  has 
 a one-to-one relationship with its **Cargo**, so it doesn't really have an identity of its own. Its identity is borrowed 
 from the **Cargo** that owns it.

 **Delivery Specification**
 Although it represents the goal of a **Cargo**, this abstraction doesn't depend on **Cargo**. It really expresses a 
 hypotheticalstate of some **Delivery History**. We hope that the **Delivery History** attached to our **Cargo** will 
 eventually satisfy the **Delivery Specification** attached to our **Cargo**. If we had two **Cargoes** going to the same 
 place, they could share the same **Delivery Specification**, but they could not share the same **Delivery History**, 
 even though the histories start ouf the same (Empty). **Delivery Speficiacations** are **Value Objects**.

 **Role and Other Attributes**  
 Role says something about the association it qualifies, but it has no history or continuity. It is a Value Object, and 
 it could be shared among different  **Cargo**/ **Customer** associations. Other attributes such as time stamps or 
 names are **Value Objects**

 ## Designing Associations in the Shipping Domain
 None of the associations in the original diagram specified a traversal direction, but bidirectional associations
 are probematic in a design. Also, traversal direction often captures insight into the domain, deepening the
 model itself.

 ![Class Diagram with Associations representing a Model of the Shipping Domain](docs/diagrams/DDD%20-%20Cargo%20-%201.1.%20Class%20Diagram%20with%20Associations%20representing%20a%20Model%20of%20the%20Shipping%20Domain.jpg?raw=true "Class Diagram with Associations representing a Model of the Shipping Domain")

[Fig 1.1 Class Diagram with Associations representing a Model of the Shipping Domain](https://miro.com/app/board/uXjVM5Gp1iE=/?moveToWidget=3458764558916533138&cot=14)


## Aggregate Boundaries

**Customer**, **Location** and **Carrier Movement** have their own identities and are shared by many **Cargoes**,
so they must be the roots of their own Aggregates. Cargo is also obvious Aggregate root, but where to draw 
the boundary takes some thought.  

The **Cargo** *Aggregate* could sweep in everything that would not exist but for particular **Cargo**, 
which could include the **Delivery History**, the **Delivery Specification**, and the **Handling Events**.   

This fits for **Delivery History**. No one would lookup a **Delivery History** directly without wanting the 
**Cargo** itself. With no need for direct global access, and with an identity that is really just derived 
from the Cargo, the **Delivery History** fits nicely inside **Cargo**'s boundry, and it does not need to be 
a root.  

The **Delivery Specification** is a Value Object, so there are no complications from including it in the **Cargo**
*Aggregate*.

The Handling Event is another matter. Previously we have considerd two possible database queries that would
search for these: 

1. To find the Handling Events for a Delivery History as possible alternate to the collection, would be local within Cargo Aggregate
2. To find all the operations to load and prepare for a particular Carrier Movement. 

In the second case, it seems that the activity of handling the Cargo has some meaning even when considerd apart
from the Cargo itself. So the Handling Event should be the root of its own Aggregate.

![Model of the Shipping Domain with Aggregate Boundaries Imposed](docs/diagrams/DDD%20-%20Cargo%20-%202.%20Model%20of%20the%20Shipping%20Domain%20with%20Aggregate%20Boundaries%20Imposed.jpg?raw=true "Model of the Shipping Domain with Aggregate Boundaries Imposed")

[Fig 2. Model of the Shipping Domain with Aggregate Boundaries Imposed](https://miro.com/app/board/uXjVM5Gp1iE=/?moveToWidget=3458764558945078621&cot=14)

## Selecting Repositories
There are five *Entities* in the design that are root of *Aggregates*, so we can limit our 
consideration to these, since none of other objects is allowed to have *Repositories*  
To decide which of these candidats should actually have a Repository, we must go back to the
application requirements. In order to take a booking through the **Booking Application**, the user
needs to select the **Customer**(s) playing the various roles (shipper, receiver and so on). So we need
a **Customer Repository**. We also need to find a Location to specify as the destination for the Cargo,
so we create a location repository. 
    The **Activity Logging Applicaiton** needs to allow the user to look up the **Carrier Movement**
that a **Cargo** is being loaded onto, so we need a **Carrier Movement Repository**. This user 
must also tell the system which **Cargo** has been loaded, so we need a **Cargo Repository**.   
For now there is no **Handling Event Repository**, because we decieded to implement the association
with **Delivery History** as a collection in the first iteration, and we have no application 
requirement to find out what has been loaded onto a **Carrier Movement**. Either of these reasons
could change; if they did, then we would add a *Repository*.

![Model Of The Shipping Domain With Repositories](docs/diagrams/DDD%20-%20Cargo%20-%203.%20Model%20of%20the%20Shipping%20Domain%20with%20Repositories.jpg?raw=true "Model Of The Shipping Domain With Repositories")
[Fig 3. Model of the Shipping Domain With Repositories](https://miro.com/app/board/uXjVM5Gp1iE=/?moveToWidget=3458764558966634663&cot=14)


## Walking Through Scenarios
To cross-check all these decisions, we have to consistently step through scenarios to confirm
that we can solve application problems effictively.

**Sample Application Feature: Changing the Destination of a Cargo**  

Occasionally, a **Customer** calls up and says, "Oh no! We said to send our cargo to Hackensack, 
but we really need it in Hoboken." We are here to server, so the system is required to provide
for this change.  
**Delivery Specification** is a *Value Object*, so it would be simplest to just to throw away and get
a new one, then use a setter method on Cargo to replace the old one with the new one.

**Sample Application Feature: Repeat Business**  

The users say that repeated bookings from the same **Customers** tend to be similar, so they want to 
use old **Cargoes** as prototype for new ones. The application will allow then to find **Cargo** in the
*Repository* and then select a command to create a new **Cargo** based on the selected one. We'll design
this using the *Prototype* pattern (Gamma et al. 1995).  
**Cargo** is an *Entity* and is the root of an *Aggregate*. Therefore, it must be copied carefully; we
need to consider what happen to each object or attribute enclosed by its *Aggregate* boundary. 
Lets go over each one:  

- **Delivery History**: We should create a new, empty one, because the history of the old one
doesn't apply. This is usual case with Entities inside the *Aggregate* boundary.  
- **Customer Roles**: We should copy the **Map** (or other collection) that holds the keyed references
to **Customers**, including the keys, because they are likely to play the same roles in the new 
shipment. But we have to be careful not to copy the Customer objects themselves. We must end up
to the same Customer objects as the old Cargo object referenced, becuase they are *Entities* 
outside the *Aggregate* boundary.

## Object Creation

**Factories and Constructors for Cargo**  
Even if we have a fancy Factory for Cargo, or use another Cargo as the Factory, as in "Repeat Business"
scenario, we still have to have a primitive constructor. We would like the constructor to produce
an object that fullfills its invariants or at least, in the case of an Entity, has its identity
intact.  
Given these decisions, we might create a Factory method on Cargo such as this: 
```
public Cargo CopyPrototype(string newTrackingId)
```
Or, we might make a method on a standalone Factory such as this: 
```
public Cargo NewCargo(Cargo prototype, string newTrackingId)
```
A standalone Factory could also encapsulate the process of obtaining a new (automatically 
generated) Id for a new Cargo, in which case it would need only one argument:
```
public Cargo NewCargo(Cargo prototype)
```
The result returned from any of these **Factories** would be the same: A **Cargo** with an empty
**Delivery History**, and a null **Delivery Specification**.  
The two way association between **Cargo** and **Delivery History** means that neither **Cargo**, nor
**Delivery History** is complete without point to its counter part, so they must be created 
together. Remember that **Cargo** is the root of the *Aggregate* that include **Delivery History**. 
Therefore, we can allow **Cargo**'s constructor or **Factory** to create a **Delivery History**. The 
**Delivery History** constructors will take a **Cargo** as an argument. The result would be something
like this:
```
public Cargo(string id)
{
  TrackingId = id;
  DeliveryHistory = new DeliveryHistory(this);
  CustomerRoles = new Dictionary<Role, Customer>();
}
```

The result is a new **Cargo** with a new **Delivery History** that points back to the **Cargo**. The
**Delivery History** constructor is used exclusively by its Aggregate root, namely **Cargo**, so 
that the composition of **Cargo** is encapsulated.

**Adding Handling Event**
Each time the cargo is handled in the real world, some user will enter a **Handling Event** 
using the **Incident Logging Application**.  
Every class must have primitive constructors. Because the **Handling Event** is an Entity, all 
attributes that define its identity must be passed to the consturctor. As discussed 
previously, the** Handling Event** is uniquely identified by the combination of the Id of its
**Cargo**, the completion time, and the event type. The only other attributes of **Handling Event**
is the association to **Carrier Movement**, which some types of H**andling Event's** don't event have.
A basic constructor that creates a valid **Handling Event** would be:
```
public HandlingEvent(Cargo cargo, string eventType, DateTimeOffset completionTime
{
    Handled = cargo;
    Type = eventType;
    CompletionTime = completionTime
}
```
Nonidentifying attributes of an *Entity* can usually be added later. In this case, 
all attributes of the **Handling Event** are going to be set in the initial transaction and
never altered(except possibly for correcting data-entry error), so it could be convinient, 
and make client code more expressive, to add a simple Factory Method to **Handling Event** for 
each event type, taking all the necessary arguments. For example, a "loading event" does
involve a **Carrier Movement**:
```
public static HandlingEvent NewLoading(Cargo cargo, CarrierMovement loadedOnto, DateTimeOffset timeStamp)
{
    HandlingEvent result = new HandlingEvent(cargo, LoadingEvent, timestamp);
    result.SetCarrierMovement(loadedOnto);
    return result;
}
```
The **Handling Event** in the model is an abstraction that might encapsulate a variety of
specialized **Handling Event** classes, ranging from loading and unloading to sealing, storing,
and other activities not related to **Carriers**. They might be implemented as multiple Subclasses 
or have complicated initialization or both. By adding Factory Methods to the base class 
(Handling Event) for each type, instace creation is abstracted, freeing the client from 
knowledge of the implementation. The *Factory* is responsible for knowing what class was to 
be instantiated and how it should be initialized.  
Unfortunately, the story isn't quite that simple. The cycle of references, from **Cargo** to 
**Delivery History** to **Handling Event** and back to **Cargo**, complicates the instance creation.
the **Delivery History** holds a collection of **Handling Events** relevent to ints **Cargo**, and the
new object must be added to this collection as part of the transaction. If this back-pointer
were not created, the objects would be inconsistent.

![Adding A Handling Event Requires Inserting It Into A Delivery History](docs/diagrams/DDD%20-%20Cargo%20-%204.%20Adding%20a%20Handling%20Event%20requires%20inserting%20it%20into%20a%20Delivery%20History.jpg)
[Fig 4. Adding A Handling Event Requires Inserting It Into A Delivery History](https://miro.com/app/board/uXjVM5Gp1iE=/?moveToWidget=3458764558996557568&cot=14)

Creation of the back-pointer could be encapsulated in the Factory (and kept in the domain
layer where it belongs), but now we will look at an alternative design that eliminates this
akward interaction altogether.

## Pause for Refactoring: An Alternative Design of the Cargo Aggregate
Modeling and design is not a constant forward process. It will grind to a halt unless
there is frequent refactoring to take advantage of new insights to improve the model
and the design.  

By now, there are couple of cumbersome aspects to this design, although it does work and 
it does reflect the model. Problems that didn't seem important when starting the design
are begining to be annoying. Let's go back to one of them and, with the benefit of hindsight,
stack the design deck in our favor.  

The need to update **Delivery History** when adding a **Handling Event** gets the **Cargo**
*Aggregate* involved in the transaction. If some other user was modifying **Cargo** at the 
same time, the **Handling Event** transaction could fail or be delayed. Entering a 
**Handling Event** is an operational activity that needs to be quick and simple, 
so an important application requirement is the ability to enter **Handling Events** 
*without contention*. This pushes us to consider a different design.  

Replacing the **Delivery History's** collection of **Handling Events** with a query 
would allow **Handling Events** to be added without raising any integrity issues outside 
its own *Aggregate*. This change would enable such transactions to complete without 
interference. If there are a lot of **Handling Events** being entered and relatively few 
queries, this design is more efficient. In fact, if a relational database is the underlying 
technology, a query was probably being used under the covers anyway to emulate the 
collection. Using query rather than a collection would also reduce the difficulty of 
maintaining consistency in the cyclical reference between **Cargo** and **Handling Event**.  

To take responsibility for the queries, we will add a *Repository* for **Handling Events**. 
The **Handling Event Repository** will support a query for the **Events** related to a 
certain **Cargo**. In addition, the *Repository* can provide query optimized to answer 
specific questions efficiently. For example, if a frequent access path is the 
**Delivery History** finding the last reported load or unload, in order to infer the 
current status of the **Cargo**, a query could be devised to return just the relevant 
**Handling Event**. And if we wanted a query to find all **Cargoes** loaded on a particular 
**Carrier Movement**, we could easily add it.

![Model Of The Shipping Domain With Repositories Refactored](docs/diagrams/DDD%20-%20Cargo%20-%205.%20Model%20of%20the%20Shipping%20Domain%20with%20Repositories%20Refactored.jpg)
[Fig 5. Implementing Delivery History's collection of Handling Events as a query makes insertion
of Handling Events simple and free of contention with the Cargo Aggregate](https://miro.com/app/board/uXjVM5Gp1iE=/?moveToWidget=3458764559058357213&cot=14)

This leaves **Delivery History** with no persistant state. At this point, there is no real need to keep it around. We could 
derive **Delivery History** itself whenever it is needed to answer some question. We can derive this object because, although
the *Entity* will be repeatedly recreated, the association with the same **Cargo** object maintains the thread of continuity
between incarnations.  

The circular reference is no longer tricky to create and maintain. The Cargo Factory will be simplified to no longer attach
an empty Delivery History to new instances. Database space can be reduced slightly, and the actual number of persistent 
objects might be reduced considerably, which is a limited resource in some object database. If the common usage pattern
is that the user seldom queries for the status of a Cargo until it arrives, then a lot of unneeded work will be avoided
together.  

On the other hand, if we are using object database, traversing an association or an explicit collection is probably much
faster than a Repository query. If the access pattern includes frequent listing of the full history, rather than the 
occasional targeted query of last position, the performance trade-off might favor the explicity collection. And remember
that the added feature ("What is on this Carrier Movement?") hasn't been requested yet, and may never be, so we don't want
to pay much for that option. 

## Modules in the Shipping Model
So far we've been looking at so few objects that modularity is not an issue. Now let's look at a little bigger part 
of the shipping model(but still simplified) to see the organization into Modules that will affect the model. 

Figure 6. shows a model neatly partitioned by a infrastructure driven pacakging. 
- The objects have been grouped according to the pattern each follows. The result is that
- objects tha conceptually have little relationship (low cohesion) are crammed together
- and associations run willy-nilly between all modules(high coupling). 
- The packages tell a story, but it is not the story of shipping

![D D D   Infrastructure Driven Modules In Shipping Model](docs/diagrams/DDD%20-%20Infrastructure%20Driven%20Modules%20in%20Shipping%20Model.jpg)
[Fig 6. Infrastructure Driven Modules In Shipping Model](https://miro.com/app/board/uXjVM5Gp1iE=/?moveToWidget=3458764559163484716&cot=14)

Instead we should be looking for cohesive concepts and focusing on the project. As with smaller scale modeling decisions
, there are many ways to do it. Figure 7. shows a straight forward one.  
The *Module* names in Figure 7 contribute to the team's language. 
- Our company does *shipping for customers* so that we can *bill* them.
- Our sales and marketing people deal with *customers*, and make aggrements with them.
- The operations people do the *shipping*, getting the cargo to its specified destination.
- The back office takes care of *billing*, submitting invoices according to the pricing in *customer's* agreement.  

That's one story I can tell with this set of modules. This intuitive breakdowns could be refined, certainly, in
successive iterations, or event replaced entirely, but it is now aiding Model Driven Design and contributing to the
Ubiquitous Language.  

![D D D   Domain Concept Driven Modules In Shipping Model](docs/diagrams/DDD%20-%20Domain%20Concept%20Driven%20Modules%20in%20Shipping%20Model.jpg)
[Fig 7. Domain Concept Driven Modules in Shipping Model](https://miro.com/app/board/uXjVM5Gp1iE=/?moveToWidget=3458764559167184911&cot=14)


