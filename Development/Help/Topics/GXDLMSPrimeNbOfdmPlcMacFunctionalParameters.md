# PRIME MAC functional parameters

<!-- cosem-attributes: 14 -->

Shows a PRIME node's assigned identities, network role and current beacon arrangement.

## When to use it

For example, check whether a meter is connected as a terminal or forwarding traffic as a switch.

## Attributes

### 1. Logical name

Identifies this object instance by its six-part OBIS code. Use it to distinguish this object from other objects of the same class.

### 2. LnId

Contains the local node identifier assigned when this node registered.

### 3. LsId

Contains the local switch identifier assigned when the node became a switch. It is not used while the node is a terminal.

### 4. SId

Identifies the switch through which this node joins the subnetwork. It does not apply to a base node.

### 5. SNa

Contains the subnetwork address to which the node is registered.

### 6. State

Reports whether the node is disconnected, a terminal, a switch or a base node.

### 7. ScpLength

Shows the length, in symbols, of the shared contention period in the current frame.

### 8. NodeHierarchyLevel

Shows how deep this node is in the subnetwork hierarchy.

### 9. BeaconSlotCount

Shows how many beacon slots are provided in the current frame.

### 10. BeaconRxSlot

Identifies the slot in which this node receives its parent switch's beacon.

### 11. BeaconTxSlot

Identifies the slot in which this node sends its own beacon, when its role requires one.

### 12. BeaconRxFrequency

Shows the number of frames between received beacons. Zero means a beacon is received in every frame.

### 13. BeaconTxFrequency

Shows the number of frames between transmitted beacons. Zero means a beacon is sent in every frame.

### 14. Capabilities

Shows supported functions, such as switching, multicast, retransmission and direct connections, as capability flags.
