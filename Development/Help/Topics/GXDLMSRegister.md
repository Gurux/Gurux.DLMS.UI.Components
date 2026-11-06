# Register

<!-- cosem-attributes: 3 -->

A register holds a measurement together with its scale and physical unit. The scale tells a reader how to convert the stored number into the displayed engineering value.

## When to use it

Use it to read an energy total, voltage or another scaled measurement.

## Attributes

### 1. Logical name

Identifies this object instance using its six-part OBIS code. Use it to distinguish this object from other instances of the same class.

### 2. Value

Contains the measurement value.

### 3. Scaler and unit

Defines the decimal exponent and unit. Multiply the stored value by 10 raised to the scaler to obtain the value in that unit.
