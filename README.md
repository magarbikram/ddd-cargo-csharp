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
