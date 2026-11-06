# Credit

<!-- cosem-attributes: 11 -->

A credit object represents one source of spendable credit, such as purchased credit, emergency credit or an allowance. Its settings control when that credit can be selected and used.

## When to use it

Use it to inspect the remaining emergency credit and the conditions under which it becomes available.

## Attributes

### 1. Logical name

Identifies this object instance using its six-part OBIS code. Use it to distinguish this object from other instances of the same class.

### 2. Current credit amount

Shows the present credit balance.

### 3. Type

Identifies the kind of credit, such as token credit or emergency credit.

### 4. Priority

Sets the order in which this credit is considered relative to other credits.

### 5. Warning threshold

Sets the balance at which a low-credit warning is raised.

### 6. Limit

Defines the lower limit to which this credit may be consumed.

### 7. Credit configuration

Controls credit behavior, including whether confirmation or other selection conditions apply.

### 8. Status

Shows the current credit state, such as available, selected or exhausted.

### 9. Preset credit amount

Defines the amount used when this credit is preset or restored under its configured rules.

### 10. Credit available threshold

Sets the level used to determine whether this credit can become available.

### 11. Period

Defines when time-based or consumption-based credit is automatically reset to the preset amount. Wildcard date and time fields allow a repeating reset; an entirely unspecified time disables this reset.
