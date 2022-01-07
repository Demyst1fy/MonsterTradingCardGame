create table "TransactionTable"
(
    ta_id       serial
        constraint transactiontable_pk
            primary key,
    ta_pid      text    not null,
    ta_cost     integer not null,
    u_username  text    not null
        constraint transactiontable_usertable_u_username_fk
            references "UserTable" (u_username),
    ta_datetime text    not null
);

alter table "TransactionTable"
    owner to postgres;

create unique index transactiontable_ta_id_uindex
    on "TransactionTable" (ta_id);

