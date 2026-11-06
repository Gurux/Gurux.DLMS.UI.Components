# Automatic answering

<!-- cosem-attributes: 6,6,7 -->

This object controls how a meter answers incoming modem calls. It can share a telephone line by limiting answering times, accepted calls and ring counts.

## When to use it

Use it to let a meter answer during a nightly reading window while leaving a shared line available during the day.

## Attributes

### 1. Logical name

The OBIS code that identifies this object instance in the meter. Use it to distinguish this instance from other objects of the same class.

### 2. Mode

Selects how incoming calls are handled, including dedicated or shared line operation and limits based on calls or successful sessions.

### 3. Listening window

Lists the start and end times of periods in which the meter can answer. Date patterns can make the periods repeat.

### 4. Status

Shows whether answering is active, inactive or locked for the current window. The next listening window can enable answering again.

### 5. Number of calls

Sets the call limit used by shared line modes. Zero means there is no limit.

### 6. Number of rings

Sets separate ring counts for answering inside and outside the listening window. A zero count disables answering for that case.

### 7. Allowed callers

Lists caller numbers accepted for normal calls or wake-up requests. Caller identification support is required.
