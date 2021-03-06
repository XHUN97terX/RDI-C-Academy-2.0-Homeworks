DROP SEQUENCE EMPIRES_SEQ;
CREATE SEQUENCE EMPIRES_SEQ AS int START WITH 10 INCREMENT BY 10;

IF OBJECT_ID('EMPIRES', 'U') IS NOT NULL DROP TABLE EMPIRES;
GO


CREATE TABLE EMPIRES 
    (EMPNO              	NUMERIC(2) NOT NULL,
     ENAME               	VARCHAR(50),
     EGOV                 	VARCHAR(50),
  CONSTRAINT EMPIRES_PRIMARY_KEY PRIMARY KEY (EMPNO));

INSERT INTO EMPIRES VALUES (NEXT VALUE FOR EMPIRES_SEQ, 'Prethoryn Scourge', 'crisis');
INSERT INTO EMPIRES VALUES (NEXT VALUE FOR EMPIRES_SEQ, 'Enlightened Exterminatus', 'Determined Exterminators');
INSERT INTO EMPIRES VALUES (NEXT VALUE FOR EMPIRES_SEQ, 'Xanyr Fanatics', 'Doctrinal Enforcers');
INSERT INTO EMPIRES VALUES (NEXT VALUE FOR EMPIRES_SEQ, 'Sentinels', 'Millitary Order');
INSERT INTO EMPIRES VALUES (NEXT VALUE FOR EMPIRES_SEQ, 'Imperium of Vetirisius Primus', 'Spritual Seekers');
INSERT INTO EMPIRES VALUES (NEXT VALUE FOR EMPIRES_SEQ, 'Sacred Hulfir Nation', 'Spritiual Seekers');
INSERT INTO EMPIRES VALUES (NEXT VALUE FOR EMPIRES_SEQ, 'Qwe''Pulci Stellar Republic', 'Honorbound Warriors');
INSERT INTO EMPIRES VALUES (NEXT VALUE FOR EMPIRES_SEQ, 'Model-16 Network', 'Determined Exterminators');
INSERT INTO EMPIRES VALUES (NEXT VALUE FOR EMPIRES_SEQ, 'Enlighetend Kingdom of U-Sana-Itcar', 'Spiritual Seekers');
INSERT INTO EMPIRES VALUES (NEXT VALUE FOR EMPIRES_SEQ, 'Khell''Zen Star Regime', 'Slaving Despots');
INSERT INTO EMPIRES VALUES (NEXT VALUE FOR EMPIRES_SEQ, 'Interstellar Caphevad Nation', 'Erudite Explorers');
INSERT INTO EMPIRES VALUES (NEXT VALUE FOR EMPIRES_SEQ, 'Yax''Kalock Confederacy', 'Federation Builders');
INSERT INTO EMPIRES VALUES (NEXT VALUE FOR EMPIRES_SEQ, 'Djunn Hegemony', 'Hegemonic Imperialists');
INSERT INTO EMPIRES VALUES (NEXT VALUE FOR EMPIRES_SEQ, 'Estwani Allied Systems', 'Spiritual Seekers');
INSERT INTO EMPIRES VALUES (NEXT VALUE FOR EMPIRES_SEQ, 'Twax''lhdar Unity', 'Hive Mind');
INSERT INTO EMPIRES VALUES (NEXT VALUE FOR EMPIRES_SEQ, 'Interstellar Uthonian State', 'Slaving Despots');
INSERT INTO EMPIRES VALUES (NEXT VALUE FOR EMPIRES_SEQ, 'Empire of Sidimatus Primus', 'Slaving Despots');
INSERT INTO EMPIRES VALUES (NEXT VALUE FOR EMPIRES_SEQ, 'Avarrian Republican Systems', 'Evangelizing Zealots');
INSERT INTO EMPIRES VALUES (NEXT VALUE FOR EMPIRES_SEQ, 'Avarrian Alliance', 'Erudite Explorers');
INSERT INTO EMPIRES VALUES (NEXT VALUE FOR EMPIRES_SEQ, 'Union of Avar', 'Federation Builders');
GO
