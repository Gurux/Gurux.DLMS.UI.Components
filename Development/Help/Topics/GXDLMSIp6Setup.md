# IPv6 setup

<!-- cosem-attributes: 10 -->

This object holds IPv6 addressing and network behavior for a meter interface. An interface can have several addresses and gateway entries.

## When to use it

Use it to configure a meter on an IPv6 network and check how its addresses are assigned.

## Attributes

### 1. Logical name

The OBIS code that identifies this object instance in the meter. Use it to distinguish this instance from other objects of the same class.

### 2. Data link layer reference

Identifies the setup object for the link that carries IPv6 traffic.

### 3. Address configuration mode

Selects how IPv6 addresses are obtained, such as automatic configuration, DHCPv6 or manual settings.

### 4. Unicast IP addresses

Lists the interface's individual IPv6 addresses, including local or global addresses.

### 5. Multicast IP addresses

Lists the IPv6 multicast groups used by the interface.

### 6. Gateway IP addresses

Lists IPv6 router addresses used to reach other networks.

### 7. Primary DNS address

Sets the preferred IPv6 DNS server for resolving host names.

### 8. Secondary DNS address

Sets an additional IPv6 DNS server for name resolution.

### 9. Traffic class

Sets the IPv6 traffic class field used to identify traffic treatment or priority.

### 10. Neighbor discovery setup

Sets the maximum router solicitation retries, the wait between retries and the interval for sending router advertisements.
