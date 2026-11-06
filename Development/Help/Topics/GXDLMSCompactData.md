# Compact data

<!-- cosem-attributes: 6,6 -->

Compact data stores captured values in a compact binary representation. A template describes the data types so a reader can decode the buffer without repeated type information.

## When to use it

Use it to collect a fixed set of measurements while reducing the size of stored or transmitted data.

## Attributes

### 1. Logical name

Identifies this object instance using its six-part OBIS code. Use it to distinguish this object from other instances of the same class.

### 2. Buffer

Contains the compact encoded captured values.

### 3. Capture objects

Lists the source objects, attribute indexes and optional element indexes to capture.

### 4. Template id

Identifies the template associated with the buffer.

### 5. Template description

Describes the structure and data types required to decode the buffer.

### 6. Capture method

Selects how captures are initiated, such as explicit capture or capture when the buffer is read.
