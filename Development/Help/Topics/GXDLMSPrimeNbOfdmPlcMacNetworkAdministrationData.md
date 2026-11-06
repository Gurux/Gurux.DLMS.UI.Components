# PRIME network administration data

<!-- cosem-attributes: 6 -->

Shows the tables used to manage PRIME multicast, forwarding and active communications.

## When to use it

For example, inspect the switch and communication tables to find the route used by a remote service node.

## Attributes

### 1. Logical name

Identifies this object instance by its six-part OBIS code. Use it to distinguish this object from other objects of the same class.

### 2. MulticastEntries

Lists multicast groups and the number of nodes associated with each group.

### 3. SwitchTable

Lists switch identifiers known to the node for forwarding within the subnetwork.

### 4. DirectTable

Lists direct connections, including source and destination identities and connection identifiers.

### 5. AvailableSwitches

Lists candidate switches that the node can hear, with connection and signal information.

### 6. Communications

Lists known communication peers and link performance information, such as received signal levels and transmission settings.
