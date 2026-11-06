# Charge

<!-- cosem-attributes: 13 -->

A charge defines how money is collected for consumption, elapsed time or a payment event. It holds the pricing settings and tracks the amounts collected.

## When to use it

Use it to configure a standing charge or to collect part of each top-up toward an outstanding balance.

## Attributes

### 1. Logical name

Identifies this object instance using its six-part OBIS code. Use it to distinguish this object from other instances of the same class.

### 2. Total amount paid

Shows the total amount collected by this charge.

### 3. Charge type

Selects whether collection is based on consumption, time or payment events.

### 4. Priority

Sets the collection priority relative to other charges.

### 5. Unit charge active

Contains the active commodity reference, scaling and price table.

### 6. Unit charge passive

Contains a replacement unit charge prepared for later activation.

### 7. Unit charge activation time

Sets when the passive unit charge becomes active.

### 8. Period

Defines the collection period in seconds for periodic charging.

### 9. Charge configuration

Sets options such as percentage-based collection and continuous collection.

### 10. Last collection time

Records when the most recent collection occurred.

### 11. Last collection amount

Shows the amount taken in the most recent collection.

### 12. Total amount remaining

Shows how much is still to be collected when collection has a fixed total.

### 13. Proportion

Sets the share of a top-up collected by a percentage-based payment-event charge. Values from 0 to 10,000 represent 0 to 100 percent.
