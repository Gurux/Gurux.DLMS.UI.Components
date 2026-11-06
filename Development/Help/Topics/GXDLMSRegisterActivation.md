# Register activation

<!-- cosem-attributes: 4 -->

Register activation selects which registers in an assigned group are active. Named masks allow different subsets to be selected without rebuilding the group.

## When to use it

Use it to select the set of tariff registers that should accumulate consumption.

## Attributes

### 1. Logical name

Identifies this object instance using its six-part OBIS code. Use it to distinguish this object from other instances of the same class.

### 2. Register assignment

Lists the register objects managed by this activation object.

### 3. Mask list

Defines named masks, each containing the indexes of registers that belong to that selection.

### 4. Active mask

Identifies the mask currently used to select active registers.
