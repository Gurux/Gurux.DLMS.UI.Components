# Network time setup

<!-- cosem-attributes: 7 -->

This object configures time synchronization with an NTP server. It defines the server endpoint and any authentication needed to trust its time responses.

## When to use it

Use it to keep a network-connected meter's clock synchronized with a central time service.

## Attributes

### 1. Logical name

The OBIS code that identifies this object instance in the meter. Use it to distinguish this instance from other objects of the same class.

### 2. Activated

Enables or disables NTP time synchronization.

### 3. Server address

Identifies the time server by a resolvable host name or a textual IPv4 or IPv6 address.

### 4. Port

Sets the server's UDP port. The usual NTP port is 123.

### 5. Authentication

Selects no authentication, shared secret authentication or the supported autokey mechanism.

### 6. Keys

Lists shared authentication keys and their identifiers for shared secret operation.

### 7. Client key

Holds the server public key used by the supported IFF autokey authentication scheme.
