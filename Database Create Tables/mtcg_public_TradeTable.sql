create table "TradeTable"
(
    tr_id            text not null
        constraint tradetable_pk
            primary key,
    c_id             text
        constraint tradetable_cardtable_c_id_fk
            references "UserCardTable",
    tr_searchtype    text,
    tr_minimumdamage double precision,
    u_username       text
        constraint tradetable_usertable_u_username_fk
            references "UserTable" (u_username)
);

alter table "TradeTable"
    owner to postgres;

create unique index tradetable_tr_id_uindex
    on "TradeTable" (tr_id);

