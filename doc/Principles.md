# Principles
## Serices
- Services don't has 
  - checks
  - buisiness logic
- Use only methods of other objects

## Model and DTO
- Validate all data
- Has business logic in first place

## IRepository
- Has business logic if it necessary
- Define all selects from database

## Validators
- Only in validators allows validate data

## Service exception
- Service Expection return [unified exception](#unified-exception)

## Unified exception
- Contain
  - EventId
    - Unique number
    - Techinical text
  - arguments
- Convertable to IActionResult and has links to exact status code

## Interfaces
- If not found allowed then 
  - Naming convention: Name contain word Try
  - Return value or Null

