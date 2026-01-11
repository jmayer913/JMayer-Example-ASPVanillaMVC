# ASP.NET Core MVC (Vanilla) Example Project

This example project is a simplified work order scheduling system and it would be part of a CMMS software (computerized maintenance management system). The example pregenerates work order templates for a BHS (baggage handling system) and a schedule for each template. A worker runs in the background and it will create work orders from the templates at 12AM each day; this also happens on startup. The example has a work order templates and work orders page.

## Work Order Templates Page

The work order templates page allows the user to add/edit/delete work order templates. A disabled schedule will automatically be created when a work order template is created. The user can enable the schedule and setup it up based on their needs. A work order can also be manually created; scheduling restrictions are ignored.

<img width="1919" height="979" alt="image" src="https://github.com/user-attachments/assets/a3eff809-2f05-48a0-9d60-51d9d95fbbe9" />

### Add / Edit

On the work order templates page, the user can create or edit an existing template.

* Name - A friendly name for the template; required and must be unique.
* Description - A description about the work order; optional.
* Service Type - The type of service that will be done; required. Options are Inspection, Routine, Reactive and Other.
* Other Type of Service - A friendly name to describe the other type of service; only enabled when the Service Type is Other.
* Priority - The urgency of completion for the work order; required. Options are Low, Normal and High.
* Days Due From Creation - When the scheduler or user manually creates a work order from this template, a due date will be set based this value; 0 means the work order will have no due date.

<img width="387" height="469" alt="image" src="https://github.com/user-attachments/assets/549ebe1a-dffb-4013-aaaf-5c34f0528ffa" />

<img width="386" height="457" alt="image" src="https://github.com/user-attachments/assets/3e0ba66a-6b2d-41a0-94e7-7a6ad35e3d94" />

### Delete

On the work order templates page, the user can delete a template. The user will be required to confirm the deletion.

<img width="568" height="413" alt="image" src="https://github.com/user-attachments/assets/18491f1c-201e-4a5f-b5dd-7065822642f8" />

### Schedule

On the work order templates page, the user can edit the schedule for a template.

* Schedule Frequency - How often the work order is created by the scheduler; required. Options are Daily, Weekly, Monthly, Quarterly, Semiyearly and Yearly.
* Start Date - The day the scheduler starts considering creating the work order for the template; required.
* End Date - The day the scheduler stops considering creating the work order for the template; optional. No selection means the schedule never ends.
* Enabled - Work orders are only created if checked. When not checked, the Schedule Frequency, Start Date, End Date and Create Work Order link will be disabled.

<img width="394" height="341" alt="image" src="https://github.com/user-attachments/assets/9dead040-04ee-47a7-bf82-ffa76980e1e5" />

<img width="401" height="339" alt="image" src="https://github.com/user-attachments/assets/d23f7871-11b3-405c-8972-e231911e673c" />

#### Create Work Order

On the schedule page, the user can manually create a work order; scheduling restrictions ignored. The user will be required to confirm the creation or go back to the scheduler page or work order template page. Once confirmed, the work order will be created and the user will be navigated to the work orders page.

<img width="709" height="407" alt="image" src="https://github.com/user-attachments/assets/12b270b2-6cca-46c1-bf94-9e4b1c03be32" />

## Work Orders Page

The work orders page is read-only so the users can view the work orders automatically created by the background scheduler or manually created by the user. The JMayer-Example-ASPSyncfusionMVC project has a work order page users can interact with.

<img width="1919" height="979" alt="image" src="https://github.com/user-attachments/assets/c010e942-d0a3-47af-86e7-50eb6d6f16bd" />

## Error Page

This page is displayed to the user when an error occurs on the server; 500 is returned.

<img width="1919" height="978" alt="image" src="https://github.com/user-attachments/assets/86c4302e-d54b-416a-8930-258a5b043768" />

## Not Found Page

This page is displayed to the user when a page or resource is not found; 404 is returned.

<img width="1919" height="980" alt="image" src="https://github.com/user-attachments/assets/043649e0-4469-43ed-a827-ac27d86a48a5" />

## Conflict Page

This page is displaye to the user when two or more users tries to edit a resource at the same time; 409 is returned.

<img width="1919" height="980" alt="image" src="https://github.com/user-attachments/assets/f69bb0c9-db56-4e86-a9a6-a4cb2956f935" />

## Background Scheduler

The background scheduler will generate works orders each day at 12 AM using the below rules.

* Only enabled template schedules are used.
* Day ran must be within the template schedule's start and end dates.
* Follow the frequency rules
  * Daily - The scheduler creates the work order each day.
  * Weekly - The scheduler creates the work order on Monday of each week.
  * Monthly - The scheduler creates the work order on the 1st of each month.
  * Quarterly - The scheduler creates the work order on the 1st of January, April, July and October.
  * Semiyearly - The scheduler creates the work order on the 1st of January and July.
  * Yearly - The scheduler creates the work order on the 1st of January.




