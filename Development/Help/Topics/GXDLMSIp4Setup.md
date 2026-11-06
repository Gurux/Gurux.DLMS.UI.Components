# IPv4 setup

<!-- cosem-attributes: 10 -->

This object holds the IPv4 network settings for a meter interface. It connects IP addressing and routing to the underlying communication link.

## When to use it

Use it to assign a meter a fixed IPv4 address or enable automatic addressing on a local network.

## Attributes

### 1. Logical name

The OBIS code that identifies this object instance in the meter. Use it to distinguish this instance from other objects of the same class.

### 2. Data link layer reference

Identifies the setup object for the link that carries IPv4 traffic.

### 3. IP address

Holds the IPv4 address assigned to this interface.

### 4. Multicast IP addresses

Lists multicast group addresses used by the interface to receive group traffic.

### 5. IP options

Holds optional IPv4 packet settings, with each option's type, length and data.

### 6. Subnet mask

Defines which part of an IPv4 address identifies the local network.

### 7. Gateway IP address

Sets the router address used to reach destinations outside the local network.

### 8. Use DHCP

Selects whether the interface obtains IPv4 configuration automatically through DHCP.

### 9. Primary DNS address

Sets the preferred DNS server used to resolve host names.

### 10. Secondary DNS address

Sets an additional DNS server for name resolution when the preferred server is unavailable.
