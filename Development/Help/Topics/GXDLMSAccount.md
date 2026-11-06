# Account

<!-- cosem-attributes: 19 -->

An account brings together the credit balances, charges and payment settings used to manage a supply contract. It provides an overall view of the money available and the debts linked to that account.

## When to use it

Use it to inspect a prepaid electricity account and see how its credits, top-ups and standing charges are coordinated.

## Attributes

### 1. Logical name

Identifies this object instance using its six-part OBIS code. Use it to distinguish this object from other instances of the same class.

### 2. Payment mode

Contains both the payment mode and the account status, such as active or closed.

### 3. Current credit in use

Gives the index in Credit references of the credit currently being used.

### 4. Current credit status

Reports flags describing the current credit situation, including conditions that affect whether supply can continue.

### 5. Available credit

Shows the combined credit currently available to the account.

### 6. Amount to clear

Shows the credit needed to clear negative balances and restore repayable credit, including the clearance threshold. This amount is represented as a negative value.

### 7. Clearance threshold

Sets the available credit level needed after clearing negative balances before repayable credits become enabled again.

### 8. Aggregated debt

Shows the sum still to collect from this account's fixed-total charges. Continuous charges are excluded.

### 9. Credit references

Lists the logical names of the Credit objects belonging to this account.

### 10. Charge references

Lists the logical names of the Charge objects belonging to this account.

### 11. Credit charge configurations

Links credits to charges and specifies the conditions under which charges may be collected from each credit.

### 12. Token gateway configurations

Defines how incoming token credit is allocated among the referenced credits.

### 13. Account activation time

Records or schedules when the account becomes active.

### 14. Account closure time

Records or schedules when the account closes.

### 15. Currency

Defines the currency name, monetary scale and currency unit used for account amounts.

### 16. Low credit threshold

Shows the warning level associated with the credit currently in use.

### 17. Next credit available threshold

Shows the availability threshold for the next credit that may become usable.

### 18. Max provision

Limits the total amount that payment-event charges can collect from top-ups during the provision period.

### 19. Max provision period

Defines the period, in seconds, over which the maximum provision limit applies.
