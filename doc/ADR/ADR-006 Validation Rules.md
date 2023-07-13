# ADR-006 Validation Rules
## Contex
Need to fix rules for:
- validation in DTO, Models. 
- service methods

### Inputs
- Model prevent set up wrong values
- Don't want to copy validation logic

## Solution
### Solution 1
#### Model validation
- Model cann't set values witch break rule of Validator
- Model contain Validator for itself
- Model.Validator use atomic Validators
  - Atomic Validators validate single field
  - Model use this atomic Validators in self methods and setters
  - Model use this atomic Validators in Constructor
#### DTO validation
- DTO can be setted into wrong state
- DTO contain Validator which Validate it
- DTO Validator use atomic Validators made by model
#### Service workflow
- Service expect that DTO will be valid
- Service raise exception on any unvalid errors or ArgumentOutOfRange for unexpected ID
#### Controller workflow
- Validate DTO usinge Validator defined in DTO
- Call service
- Catch all exceptions and return Unexpected error info for them
