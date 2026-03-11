-- Add ON DELETE CASCADE so deleting a customer also deletes their bookings/tickets
-- Run in SQL Developer as KumariCinema
-- ORA-02292 fix: child records will be deleted automatically with parent

-- If constraint name differs, run: SELECT constraint_name FROM user_constraints WHERE table_name='BOOKING' AND constraint_type='R';

ALTER TABLE BOOKING DROP CONSTRAINT FK_BOOKING_CUSTOMER;
ALTER TABLE BOOKING ADD CONSTRAINT FK_BOOKING_CUSTOMER 
  FOREIGN KEY (CUSTOMERID) REFERENCES CUSTOMER(CUSTOMERID) ON DELETE CASCADE;
