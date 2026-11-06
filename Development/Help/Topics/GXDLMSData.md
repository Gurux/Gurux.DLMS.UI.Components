# Data

<!-- cosem-attributes: 2 -->

A Data object holds a value whose meaning is identified by its logical name. The value may be a simple item or a structured collection.

## When to use it

Use it to read a device identifier, a configuration value or another item that does not need register scaling.

## Attributes

### 1. Logical name

Identifies this object instance using its six-part OBIS code. Use it to distinguish this object from other instances of the same class.

### 2. Value

Contains the stored data. Its type and meaning depend on the particular object instance.

For a Unix-time instance, the value is the number of seconds since 1 January 1970 at 00:00 UTC. The Unix-time editor shows that instant in the local time zone; other Data instances can hold entirely different kinds of values.
