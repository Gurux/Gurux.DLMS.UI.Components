# Profile generic

<!-- cosem-attributes: 8,8 -->

A profile stores rows of captured object values. Its capture list defines the columns, while capture and sorting settings control how rows are collected and retained.

## When to use it

Use it to read a load profile containing timestamps, energy readings and status values.

## Attributes

### 1. Logical name

Identifies this object instance using its six-part OBIS code. Use it to distinguish this object from other instances of the same class.

### 2. Buffer

Contains the stored rows of captured values.

### 3. Capture objects

Defines the columns by listing each source object class, logical name, attribute index and data index. The data index can select an element of a structured attribute; zero selects the complete attribute.

### 4. Capture period

Sets the automatic capture interval in seconds. A value of zero disables periodic capture.

### 5. Sort method

Selects the ordering and retention rule, such as first-in first-out or sorting by a captured value.

### 6. Sort object

Identifies the sorting key by object class, logical name, attribute index and data index.

### 7. Entries in use

Shows the number of rows currently stored.

### 8. Profile entries

Defines the maximum number of rows the profile can hold.

## Versions

Versions 0 and 1 use the same attribute indexes and names. Use the object version and the device-supported access selections when retrieving profile rows.
