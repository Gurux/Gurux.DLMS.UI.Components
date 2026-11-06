# TCP and UDP setup

<!-- cosem-attributes: 6 -->

This object configures the transport endpoint used for meter communication over an IP network. It links the listening port to IP settings and defines connection limits.

## When to use it

Use it when setting the port on which a meter accepts remote connections and deciding how long idle TCP sessions can remain open.

## Attributes

### 1. Logical name

The OBIS code that identifies this object instance in the meter. Use it to distinguish this instance from other objects of the same class.

### 2. Port

Sets the TCP or UDP listening port for meter communication, or the UDP endpoint used by CoAP.

### 3. IP reference

Identifies the IPv4 or IPv6 setup object that supplies the underlying network configuration.

### 4. Maximum segment size

Advertises the maximum TCP receive segment size to the other endpoint during connection establishment.

### 5. Maximum simultaneous connections

Limits how many transport connections the meter can support at once.

### 6. Inactivity timeout

Sets how many seconds without received traffic cause an idle TCP connection to close. Zero disables this timeout.
