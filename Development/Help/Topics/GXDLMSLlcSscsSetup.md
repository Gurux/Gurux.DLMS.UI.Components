# LLC SSCS setup

<!-- cosem-attributes: 3 -->

Shows the addresses assigned when a service node registers with a base node on a power line network.

## When to use it

For example, read both addresses to confirm that a newly installed meter has registered with the intended base node.

## Attributes

### 1. Logical name

Identifies this object instance by its six-part OBIS code. Use it to distinguish this object from other objects of the same class.

### 2. ServiceNodeAddress

Contains the service node address assigned during registration. After deregistration it returns to the unassigned NEW address.

### 3. BaseNodeAddress

Contains the address of the base node with which this node is registered. A zero value indicates that it is not registered.
