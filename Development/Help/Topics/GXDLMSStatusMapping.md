# Status mapping

<!-- cosem-attributes: 3 -->

This object explains the bits in a meter status word by linking them to entries in a status reference table. It separates the current status value from the meaning of each bit.

## When to use it

Use it to interpret an alarm or event status word whose bits refer to a shared status table.

## Attributes

### 1. Logical name

The OBIS code that identifies this object instance in the meter. Use it to distinguish this instance from other objects of the same class.

### 2. Status word

Holds the current status value. Its contents are interpreted as bits even when the value is carried in a number or string.

### 3. Mapping table

Identifies the reference status table and maps bit positions to its entries. A starting entry can describe consecutive mappings, or a list can map each bit separately.
