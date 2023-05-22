# ADR-001 Decimal in candle value

## Context
1. No lazy loading
2. It is nessary to compare on init

## Requirements
1. Need to store decimal with different decimal len part
2. Must contain strict values
3. Easy to convert

## Solutions
### Split on two fields and len store in Instrument entity
+ Solve problems
- Problem with initing values because of Context-1
- **Problem to comparing (Context-2): until load Instrument entity because of Context-1**

### Store Len in Candles
+ Solve Context-1 and Context-2
- **Increase memory usage**

### Store without comma and len store in Instument entity
+ Solve problem
+ Could compare values with out knowledge of len
- Get restriction that int value must contain following zeros to get right len of decimal values
  1.2 with decimal len 2 => 120