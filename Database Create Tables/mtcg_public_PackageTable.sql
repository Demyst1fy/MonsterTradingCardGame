create table "PackageTable"
(
    p_id      text             not null,
    p_cid     text             not null
        constraint packagetable_pk
            primary key,
    p_cname   text             not null,
    p_cdamage double precision not null
);

alter table "PackageTable"
    owner to postgres;

create unique index packagetable_p_cid_uindex
    on "PackageTable" (p_cid);

