# LTE monitoring

<!-- cosem-attributes: 2,3 -->

<!-- versions: 0 -->
This object reports LTE network timers and radio service quality.
<!-- /versions -->

<!-- versions: 1 -->
This object reports LTE network parameters and radio service quality, including LTE Cat M1 and narrowband IoT operation.
<!-- /versions -->

## When to use it

Use it to investigate weak cellular coverage or understand a battery-powered meter's network sleep and wake timing.

## Attributes

### 1. Logical name

The OBIS code that identifies this object instance in the meter. Use it to distinguish this instance from other objects of the same class.

<!-- versions: 0 -->
### 2. Quality of service (version 0)

Reports network timers T3402 and T3412, received signal quality RSRQ, received signal strength RSRP and the minimum required reception level qRxlevMin.
<!-- /versions -->

<!-- versions: 1 -->
### 2. Network parameters (version 1)

Reports network timers and parameters for power saving, extended paging and coverage, including T3402, T3412 extended, T3324, extended DRX, paging time window and qRxlevMin.
<!-- /versions -->

<!-- versions: 1 -->
### 3. Quality of service (version 1)

Reports radio quality RSRQ, received signal level RSRP, signal-to-noise information and the coverage enhancement mode.
<!-- /versions -->

