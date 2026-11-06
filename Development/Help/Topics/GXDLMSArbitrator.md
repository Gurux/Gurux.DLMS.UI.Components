# Arbitrator

<!-- cosem-attributes: 6 -->

An arbitrator resolves competing requests for a set of actions. Permissions and priorities determine which requested action is selected.

## When to use it

Use it when several internal controls can request changes to the same supply switch.

## Attributes

### 1. Logical name

Identifies this object instance using its six-part OBIS code. Use it to distinguish this object from other instances of the same class.

### 2. Actions

Lists the script actions that can be selected.

### 3. Permissions table

Defines which actions each requesting actor is permitted to request.

### 4. Weightings table

Assigns weights to actors and actions so competing requests can be compared.

### 5. Most recent requests table

Shows the latest action requests received from the actors.

### 6. Last outcome

Identifies the action selected by the latest arbitration result.
