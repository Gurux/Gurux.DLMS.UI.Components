# Utility tables

<!-- cosem-attributes: 4 -->

Utility tables exposes a table identified by a table number as a sequence of bytes. The meaning of those bytes belongs to the table format used by the device.

## When to use it

Use it to read a utility-specific table that is transported through a standard object interface.

## Attributes

### 1. Logical name

Identifies this object instance using its six-part OBIS code. Use it to distinguish this object from other instances of the same class.

### 2. Table id

Identifies the table represented by this object.

### 3. Length

Shows the table length in bytes.

### 4. Buffer

Contains the table data as bytes.
