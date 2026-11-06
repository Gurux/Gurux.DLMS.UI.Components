# G3-PLC 6LoWPAN setup

<!-- cosem-attributes: 16,19,21,23 -->

Controls how IPv6 traffic is carried and routed over a G3 power line network. It includes route discovery, network membership and link security settings.

## When to use it

For example, inspect the routing table when a meter can reach a nearby meter but cannot reach the data concentrator.

## Attributes

### 1. Logical name

Identifies this object instance by its six-part OBIS code. Use it to distinguish this object from other objects of the same class.

### 2. MaxHops

Limits how many forwarding steps a packet may take before it is discarded.

### 3. WeakLqiValue

Sets the link quality level below which a neighbouring link is treated as weak during routing.

### 4. SecurityLevel

Selects the protection applied to adaptation layer traffic, such as authentication and encryption.

### 5. PrefixTable

Lists IPv6 prefixes that can be shortened when packet headers are compressed.

### 6. RoutingConfiguration

Sets route discovery and route maintenance parameters, including timing, retries and route cost calculation.

### 7. BroadcastLogTableEntryTtl

Sets how long received broadcast identifiers remain in the duplicate detection log.

### 8. RoutingTable

Lists known destinations, the next hop towards each one, route cost and remaining route lifetime.

### 9. ContextInformationTable

Stores the IPv6 compression contexts used to shorten packet headers and their remaining lifetimes.

### 10. BlacklistTable

Lists neighbours temporarily excluded from routing and how long each exclusion lasts.

### 11. BroadcastLogTable

Records recently seen broadcast sources and sequence numbers so repeated copies are discarded.

### 12. GroupTable

Lists the multicast groups to which the device belongs.

### 13. MaxJoinWaitTime

Limits how long a device waits for a response while joining the network.

### 14. PathDiscoveryTime

Sets the time allowed for discovering a route to a destination, in seconds.

### 15. ActiveKeyIndex

Selects the group master key used to protect outgoing traffic.

### 16. MetricType

Selects the method used to calculate and compare route costs.

### 17. CoordShortAddress

Contains the short network address of the coordinator.

### 18. DisableDefaultRouting

Controls whether the normal LOADng route discovery mechanism is disabled.

### 19. DeviceType

Identifies the device role, such as a network device or the network coordinator.

### 20. Default coord route enabled

Controls whether joining the network automatically creates a default route to the coordinator.

### 21. Destination address

Lists destination addresses for devices whose connectivity is provided by this router.

### 22. Low LQI

Sets the link quality boundary below which a link is regarded as unreliable when calculating route cost.

### 23. High LQI

Sets the link quality boundary above which a link is regarded as reliable when calculating route cost.

## Versions

Version 0 provides attributes 1-16. Version 1 provides attributes 1-19. Version 2 adds attributes 20-21, and version 3 adds attributes 22-23.

The installed component uses the version 1 field meanings for attributes 4, 5, 14, 15 and 16 in version 0 as well. It does not implement the original version 0 tone mask, discovery wait, millisecond path discovery timer or group master key transition timers at those indexes.
