# PPP setup

<!-- cosem-attributes: 5 -->

This object configures a point-to-point connection that can carry IP communication. It links the physical port to link negotiation, IP negotiation and authentication settings.

## When to use it

Use it when a meter reaches an IP network through a modem or another point-to-point link.

## Attributes

### 1. Logical name

The OBIS code that identifies this object instance in the meter. Use it to distinguish this instance from other objects of the same class.

### 2. Physical layer reference

Identifies the setup object for the physical interface that carries the PPP connection.

### 3. LCP options

Lists link negotiation options, such as receive size, authentication protocol and compression settings.

### 4. IPCP options

Lists options negotiated for IP communication over the PPP link, such as IP addressing.

### 5. PPP authentication

Holds the user name and password used when authentication is required for the PPP connection.
