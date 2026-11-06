# Script table

<!-- cosem-attributes: 2 -->

A script table stores named sequences of operations on other objects. Schedules, monitors and other controls can refer to a script by its selector.

## When to use it

Use it to define the actions required for a tariff change or billing-period reset.

## Attributes

### 1. Logical name

Identifies this object instance using its six-part OBIS code. Use it to distinguish this object from other instances of the same class.

### 2. Scripts

Lists the script selectors and their ordered actions. Each action identifies the target object, attribute or method, and the parameter to use.
