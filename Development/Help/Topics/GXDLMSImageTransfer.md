# Image transfer

<!-- cosem-attributes: 7 -->

This object manages downloading and activating a software or firmware image in a meter. It exposes block transfer progress separately from image verification and activation.

## When to use it

Use it to deliver a firmware update, resume missing blocks and check that the image is ready before activation.

## Attributes

### 1. Logical name

The OBIS code that identifies this object instance in the meter. Use it to distinguish this instance from other objects of the same class.

### 2. Image block size

Sets the size in bytes of the blocks used to transfer the image. The final block may be shorter.

### 3. Transferred blocks status

Shows which image blocks have been received, with one bit for each block.

### 4. First not transferred block number

Identifies the first block still missing, helping a client resume an interrupted transfer.

### 5. Image transfer enabled

Controls whether image transfer operations are allowed.

### 6. Image transfer status

Reports the current stage or result of transfer, verification and activation.

### 7. Image activation information

Lists images available for activation, with their size, identifier and signature information.
