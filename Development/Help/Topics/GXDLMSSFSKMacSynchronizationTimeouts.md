# S-FSK synchronisation timeouts

<!-- cosem-attributes: 5 -->

Configures when an S-FSK device selects an initiator or abandons an inactive or invalid connection.

## When to use it

For example, set a recovery timeout so a meter eventually searches again when its concentrator stops communicating.

## Attributes

### 1. Logical name

Identifies this object instance by its six-part OBIS code. Use it to distinguish this object from other objects of the same class.

### 2. SearchInitiatorTimeout

Sets how many seconds the device searches before selecting the initiator with the strongest signal. Zero disables this timed search feature.

### 3. SynchronizationConfirmationTimeout

Sets how many seconds a newly detected physical frame may remain unconfirmed by a valid MAC frame before synchronisation is lost.

### 4. TimeOutNotAddressed

Sets how many minutes the device may remain without an individually addressed message before returning to its unconfigured state.

### 5. TimeOutFrameNotOK

Sets how many seconds the device may remain without a correctly formed MAC frame before losing frame synchronisation.
