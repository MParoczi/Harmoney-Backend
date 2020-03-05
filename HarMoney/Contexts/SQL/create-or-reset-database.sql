DROP TABLE IF EXISTS public.transaction;
DROP SEQUENCE IF EXISTS public.transaction_id_seq;
CREATE TABLE transaction (
    id serial PRIMARY KEY,
    title VARCHAR(30),
    due_date DATE NOT NULL,
    amount INT NOT NULL,
    frequency VARCHAR(7) NOT NULL,
    direction VARCHAR(11) NOT NULL
);

INSERT INTO public.transaction (title, due_date, amount, frequency, direction)
    VALUES ('zsebpénz', TO_DATE('03/03/2020', 'DD/MM/YYYY'), 20000, 'Single', 'Income'),
           ('gázszámla', TO_DATE('15/03/2020', 'DD/MM/YYYY'), 13458, 'Monthly', 'Expenditure'),
           ('fizetés', TO_DATE('01/03/2020', 'DD/MM/YYYY'), 300000, 'Monthly', 'Income'),
           ('közös költség', TO_DATE('09/03/2020', 'DD/MM/YYYY'), 28236, 'Monthly', 'Expenditure');