# S-FSK physical and MAC setup

<!-- cosem-attributes: 15 -->

Configures the radio frequency signalling used over the power line and the device's link layer addressing and repeater behaviour.

## When to use it

For example, check signalling frequencies and the initiator address when a meter cannot synchronise with its concentrator.

## Attributes

### 1. Logical name

Identifies this object instance by its six-part OBIS code. Use it to distinguish this object from other objects of the same class.

### 2. InitiatorElectricalPhase

Identifies the mains electrical phase used by the initiator.

### 3. DeltaElectricalPhase

Shows the electrical phase difference between this device and its initiator.

### 4. MaxReceivingGain

Limits the amplification used when receiving power line signals.

### 5. MaxTransmittingGain

Limits the signal gain used when transmitting onto the power line.

### 6. SearchInitiatorThreshold

Sets the received signal level that allows a sufficiently strong initiator to be selected promptly.

### 7. Frequency

Sets the two signalling frequencies used for mark and space symbols.

### 8. MacAddress

Contains the individual MAC address used to select this device.

### 9. MacGroupAddresses

Lists group MAC addresses for which this device accepts traffic.

### 10. Repeater

Controls whether the device may act as a repeater, including automatic repeater selection where supported.

### 11. RepeaterStatus

Reports whether the device is currently acting as a repeater.

### 12. MinDeltaCredit

Records the smallest difference between initial and remaining forwarding credit seen in valid addressed frames. It helps indicate the repetition distance from the sender.

### 13. InitiatorMacAddress

Contains the MAC address of the initiator used for synchronisation.

### 14. SynchronizationLocked

Reports whether the device is locked to its initiator for synchronisation.

### 15. TransmissionSpeed

Selects the bit rate used for S-FSK transmissions.
