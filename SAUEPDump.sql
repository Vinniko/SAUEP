PGDMP             
            y         	   SAUEPOLTP    9.6.18    9.6.18 &    x           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                       false            y           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                       false            z           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                       false            {           1262    26030 	   SAUEPOLTP    DATABASE     �   CREATE DATABASE "SAUEPOLTP" WITH TEMPLATE = template0 ENCODING = 'UTF8' LC_COLLATE = 'Russian_Russia.1251' LC_CTYPE = 'Russian_Russia.1251';
    DROP DATABASE "SAUEPOLTP";
             postgres    false                        2615    2200    public    SCHEMA        CREATE SCHEMA public;
    DROP SCHEMA public;
             postgres    false            |           0    0    SCHEMA public    COMMENT     6   COMMENT ON SCHEMA public IS 'standard public schema';
                  postgres    false    3                        3079    12387    plpgsql 	   EXTENSION     ?   CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;
    DROP EXTENSION plpgsql;
                  false            }           0    0    EXTENSION plpgsql    COMMENT     @   COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';
                       false    1            �            1259    26049    devicegroup    TABLE     _   CREATE TABLE public.devicegroup (
    id integer NOT NULL,
    title character varying(100)
);
    DROP TABLE public.devicegroup;
       public         postgres    false    3            �            1259    26047    devicegroup_id_seq    SEQUENCE     {   CREATE SEQUENCE public.devicegroup_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 )   DROP SEQUENCE public.devicegroup_id_seq;
       public       postgres    false    3    188            ~           0    0    devicegroup_id_seq    SEQUENCE OWNED BY     I   ALTER SEQUENCE public.devicegroup_id_seq OWNED BY public.devicegroup.id;
            public       postgres    false    187            �            1259    26057    devices    TABLE     '  CREATE TABLE public.devices (
    id integer NOT NULL,
    devicegroupid integer,
    serial character varying(50),
    title character varying(50),
    ip character varying(25),
    port character varying(10),
    status boolean,
    maxpower double precision,
    minpower double precision
);
    DROP TABLE public.devices;
       public         postgres    false    3            �            1259    26055    devices_id_seq    SEQUENCE     w   CREATE SEQUENCE public.devices_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 %   DROP SEQUENCE public.devices_id_seq;
       public       postgres    false    3    190                       0    0    devices_id_seq    SEQUENCE OWNED BY     A   ALTER SEQUENCE public.devices_id_seq OWNED BY public.devices.id;
            public       postgres    false    189            �            1259    26033    polls    TABLE     �   CREATE TABLE public.polls (
    id integer NOT NULL,
    deviceid integer NOT NULL,
    date timestamp without time zone NOT NULL,
    power double precision,
    electricityconsumption double precision
);
    DROP TABLE public.polls;
       public         postgres    false    3            �            1259    26031    polls_id_seq    SEQUENCE     u   CREATE SEQUENCE public.polls_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 #   DROP SEQUENCE public.polls_id_seq;
       public       postgres    false    186    3            �           0    0    polls_id_seq    SEQUENCE OWNED BY     =   ALTER SEQUENCE public.polls_id_seq OWNED BY public.polls.id;
            public       postgres    false    185            �            1259    28367    roles    TABLE     a   CREATE TABLE public.roles (
    id integer NOT NULL,
    title character varying(25) NOT NULL
);
    DROP TABLE public.roles;
       public         postgres    false    3            �            1259    28365    roles_id_seq    SEQUENCE     u   CREATE SEQUENCE public.roles_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 #   DROP SEQUENCE public.roles_id_seq;
       public       postgres    false    194    3            �           0    0    roles_id_seq    SEQUENCE OWNED BY     =   ALTER SEQUENCE public.roles_id_seq OWNED BY public.roles.id;
            public       postgres    false    193            �            1259    28359    users    TABLE     �   CREATE TABLE public.users (
    id integer NOT NULL,
    login character varying(50) NOT NULL,
    password character varying(100) NOT NULL,
    email character varying(50),
    roleid integer
);
    DROP TABLE public.users;
       public         postgres    false    3            �            1259    28357    users_id_seq    SEQUENCE     u   CREATE SEQUENCE public.users_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 #   DROP SEQUENCE public.users_id_seq;
       public       postgres    false    3    192            �           0    0    users_id_seq    SEQUENCE OWNED BY     =   ALTER SEQUENCE public.users_id_seq OWNED BY public.users.id;
            public       postgres    false    191            �           2604    26052    devicegroup id    DEFAULT     p   ALTER TABLE ONLY public.devicegroup ALTER COLUMN id SET DEFAULT nextval('public.devicegroup_id_seq'::regclass);
 =   ALTER TABLE public.devicegroup ALTER COLUMN id DROP DEFAULT;
       public       postgres    false    187    188    188            �           2604    26060 
   devices id    DEFAULT     h   ALTER TABLE ONLY public.devices ALTER COLUMN id SET DEFAULT nextval('public.devices_id_seq'::regclass);
 9   ALTER TABLE public.devices ALTER COLUMN id DROP DEFAULT;
       public       postgres    false    190    189    190            �           2604    26036    polls id    DEFAULT     d   ALTER TABLE ONLY public.polls ALTER COLUMN id SET DEFAULT nextval('public.polls_id_seq'::regclass);
 7   ALTER TABLE public.polls ALTER COLUMN id DROP DEFAULT;
       public       postgres    false    186    185    186            �           2604    28370    roles id    DEFAULT     d   ALTER TABLE ONLY public.roles ALTER COLUMN id SET DEFAULT nextval('public.roles_id_seq'::regclass);
 7   ALTER TABLE public.roles ALTER COLUMN id DROP DEFAULT;
       public       postgres    false    194    193    194            �           2604    28362    users id    DEFAULT     d   ALTER TABLE ONLY public.users ALTER COLUMN id SET DEFAULT nextval('public.users_id_seq'::regclass);
 7   ALTER TABLE public.users ALTER COLUMN id DROP DEFAULT;
       public       postgres    false    192    191    192            �           2606    26054    devicegroup devicegroup_pkey 
   CONSTRAINT     Z   ALTER TABLE ONLY public.devicegroup
    ADD CONSTRAINT devicegroup_pkey PRIMARY KEY (id);
 F   ALTER TABLE ONLY public.devicegroup DROP CONSTRAINT devicegroup_pkey;
       public         postgres    false    188    188            �           2606    28396 !   devicegroup devicegroup_title_key 
   CONSTRAINT     ]   ALTER TABLE ONLY public.devicegroup
    ADD CONSTRAINT devicegroup_title_key UNIQUE (title);
 K   ALTER TABLE ONLY public.devicegroup DROP CONSTRAINT devicegroup_title_key;
       public         postgres    false    188    188            �           2606    26062    devices devices_pkey 
   CONSTRAINT     R   ALTER TABLE ONLY public.devices
    ADD CONSTRAINT devices_pkey PRIMARY KEY (id);
 >   ALTER TABLE ONLY public.devices DROP CONSTRAINT devices_pkey;
       public         postgres    false    190    190            �           2606    28398    devices devices_serial_key 
   CONSTRAINT     W   ALTER TABLE ONLY public.devices
    ADD CONSTRAINT devices_serial_key UNIQUE (serial);
 D   ALTER TABLE ONLY public.devices DROP CONSTRAINT devices_serial_key;
       public         postgres    false    190    190            �           2606    26038    polls polls_pkey 
   CONSTRAINT     N   ALTER TABLE ONLY public.polls
    ADD CONSTRAINT polls_pkey PRIMARY KEY (id);
 :   ALTER TABLE ONLY public.polls DROP CONSTRAINT polls_pkey;
       public         postgres    false    186    186            �           2606    28372    roles roles_pkey 
   CONSTRAINT     N   ALTER TABLE ONLY public.roles
    ADD CONSTRAINT roles_pkey PRIMARY KEY (id);
 :   ALTER TABLE ONLY public.roles DROP CONSTRAINT roles_pkey;
       public         postgres    false    194    194            �           2606    28400    roles roles_title_key 
   CONSTRAINT     Q   ALTER TABLE ONLY public.roles
    ADD CONSTRAINT roles_title_key UNIQUE (title);
 ?   ALTER TABLE ONLY public.roles DROP CONSTRAINT roles_title_key;
       public         postgres    false    194    194            �           2606    28379    users users_login_key 
   CONSTRAINT     Q   ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_login_key UNIQUE (login);
 ?   ALTER TABLE ONLY public.users DROP CONSTRAINT users_login_key;
       public         postgres    false    192    192            �           2606    28364    users users_pkey 
   CONSTRAINT     N   ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);
 :   ALTER TABLE ONLY public.users DROP CONSTRAINT users_pkey;
       public         postgres    false    192    192                        2606    28373    users users_fk0    FK CONSTRAINT     m   ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_fk0 FOREIGN KEY (roleid) REFERENCES public.roles(id);
 9   ALTER TABLE ONLY public.users DROP CONSTRAINT users_fk0;
       public       postgres    false    192    194    2045           