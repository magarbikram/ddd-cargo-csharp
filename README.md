# ddd-cargo-csharp
Cargo System built with DDD pattern. Summary and code from blue book [Domain Driven Design by Eric Evans](https://a.co/d/fRmp7M7)

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