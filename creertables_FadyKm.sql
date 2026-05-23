-- Travail 2 - Partie 1 : Création des tables PostgreSQL
-- Projet : Gestion d'un refuge d'animaux
-- Nom : Fady KM

DROP TABLE IF EXISTS vaccination, animal_couleur, ani_compatibilite, famille_accueil,
    adoption, ani_sortie, ani_entree, personne_role, contact, role_refuge,
    vaccin, compatibilite, couleur, animal CASCADE;

CREATE TABLE animal (
    identifiant CHAR(11) PRIMARY KEY,
    nom VARCHAR(80) NOT NULL CHECK (length(trim(nom)) >= 2),
    type VARCHAR(10) NOT NULL CHECK (type IN ('chat', 'chien')),
    sexe CHAR(1) NOT NULL CHECK (sexe IN ('M', 'F')),
    particularites TEXT,
    date_deces DATE,
    description TEXT,
    date_sterilisation DATE,
    sterilise BOOLEAN NOT NULL DEFAULT FALSE,
    date_naissance DATE NOT NULL,
    CONSTRAINT chk_animal_id CHECK (identifiant ~ '^[0-9]{11}$'),
    CONSTRAINT chk_sterilisation CHECK (
        (sterilise = FALSE AND date_sterilisation IS NULL)
        OR (sterilise = TRUE AND date_sterilisation IS NOT NULL AND date_sterilisation >= date_naissance)
    ),
    CONSTRAINT chk_deces CHECK (date_deces IS NULL OR date_deces >= date_naissance),
    CONSTRAINT chk_naissance CHECK (date_naissance <= CURRENT_DATE)
);

CREATE TABLE couleur (
    id_couleur SERIAL PRIMARY KEY,
    nom_couleur VARCHAR(40) NOT NULL UNIQUE
);

CREATE TABLE animal_couleur (
    ani_identifiant CHAR(11) NOT NULL REFERENCES animal(identifiant) ON DELETE CASCADE,
    id_couleur INTEGER NOT NULL REFERENCES couleur(id_couleur),
    PRIMARY KEY (ani_identifiant, id_couleur)
);

CREATE TABLE compatibilite (
    id_compatibilite SERIAL PRIMARY KEY,
    type VARCHAR(30) NOT NULL UNIQUE CHECK (type IN ('chat', 'chien', 'jeune enfant', 'enfant', 'jardin', 'poney'))
);

CREATE TABLE ani_compatibilite (
    ani_identifiant CHAR(11) NOT NULL REFERENCES animal(identifiant) ON DELETE CASCADE,
    id_compatibilite INTEGER NOT NULL REFERENCES compatibilite(id_compatibilite),
    valeur VARCHAR(10) NOT NULL CHECK (valeur IN ('oui', 'non', 'non testé')),
    description TEXT,
    PRIMARY KEY (ani_identifiant, id_compatibilite)
);

CREATE TABLE contact (
    contact_identifiant SERIAL PRIMARY KEY,
    nom VARCHAR(80) NOT NULL CHECK (length(trim(nom)) >= 2),
    prenom VARCHAR(80) NOT NULL CHECK (length(trim(prenom)) >= 2),
    registre_national VARCHAR(15) UNIQUE CHECK (registre_national IS NULL OR registre_national ~ '^[0-9]{2}\.[0-9]{2}\.[0-9]{2}-[0-9]{3}\.[0-9]{2}$'),
    rue VARCHAR(120),
    cp VARCHAR(10),
    localite VARCHAR(80),
    gsm VARCHAR(25),
    telephone VARCHAR(25),
    email VARCHAR(120),
    CONSTRAINT chk_moyen_contact CHECK (gsm IS NOT NULL OR telephone IS NOT NULL OR email IS NOT NULL),
    CONSTRAINT chk_email CHECK (email IS NULL OR email ~* '^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$')
);

CREATE TABLE role_refuge (
    rol_identifiant SERIAL PRIMARY KEY,
    rol_nom VARCHAR(30) NOT NULL UNIQUE CHECK (rol_nom IN ('benevole', 'adoptant', 'candidat', 'famille_accueil', 'autres'))
);

CREATE TABLE personne_role (
    pers_identifiant INTEGER NOT NULL REFERENCES contact(contact_identifiant) ON DELETE CASCADE,
    rol_identifiant INTEGER NOT NULL REFERENCES role_refuge(rol_identifiant),
    PRIMARY KEY (pers_identifiant, rol_identifiant)
);

CREATE TABLE ani_entree (
    id_entree SERIAL PRIMARY KEY,
    raison VARCHAR(30) NOT NULL CHECK (raison IN ('abandon', 'errant', 'deces_proprietaire', 'saisie', 'retour_adoption', 'retour_famille_accueil')),
    date_entree DATE NOT NULL,
    ani_identifiant CHAR(11) NOT NULL REFERENCES animal(identifiant) ON DELETE CASCADE,
    entree_contact INTEGER REFERENCES contact(contact_identifiant),
    CONSTRAINT uq_entree_animal_date UNIQUE (ani_identifiant, date_entree)
);

CREATE TABLE ani_sortie (
    id_sortie SERIAL PRIMARY KEY,
    raison VARCHAR(30) NOT NULL CHECK (raison IN ('adoption', 'retour_proprietaire', 'deces_animal', 'famille_accueil')),
    date_sortie DATE NOT NULL,
    ani_identifiant CHAR(11) NOT NULL REFERENCES animal(identifiant) ON DELETE CASCADE,
    sortie_contact INTEGER REFERENCES contact(contact_identifiant),
    CONSTRAINT uq_sortie_animal_date UNIQUE (ani_identifiant, date_sortie)
);

CREATE TABLE adoption (
    id_adoption SERIAL PRIMARY KEY,
    statut VARCHAR(30) NOT NULL CHECK (statut IN ('demande', 'acceptee', 'rejet_environnement', 'rejet_comportement')),
    date_demande DATE NOT NULL DEFAULT CURRENT_DATE,
    ani_identifiant CHAR(11) NOT NULL REFERENCES animal(identifiant) ON DELETE CASCADE,
    adop_contact INTEGER NOT NULL REFERENCES contact(contact_identifiant)
);

CREATE TABLE famille_accueil (
    id_accueil SERIAL PRIMARY KEY,
    date_debut DATE NOT NULL,
    date_fin DATE,
    fa_ani_identifiant CHAR(11) NOT NULL REFERENCES animal(identifiant) ON DELETE CASCADE,
    fa_contact INTEGER NOT NULL REFERENCES contact(contact_identifiant),
    CONSTRAINT chk_dates_accueil CHECK (date_fin IS NULL OR date_fin >= date_debut)
);

CREATE UNIQUE INDEX ux_un_accueil_actif_par_animal
ON famille_accueil(fa_ani_identifiant)
WHERE date_fin IS NULL;

CREATE TABLE vaccin (
    id_vaccin SERIAL PRIMARY KEY,
    nom VARCHAR(80) NOT NULL UNIQUE
);

CREATE TABLE vaccination (
    id_vaccination SERIAL PRIMARY KEY,
    vaccination_date DATE NOT NULL,
    vac_animal CHAR(11) NOT NULL REFERENCES animal(identifiant) ON DELETE CASCADE,
    id_vaccin INTEGER NOT NULL REFERENCES vaccin(id_vaccin),
    fait BOOLEAN NOT NULL DEFAULT TRUE,
    CONSTRAINT uq_vaccin_jour UNIQUE (vac_animal, id_vaccin, vaccination_date)
);

-- Données de base
INSERT INTO role_refuge(rol_nom) VALUES
('benevole'), ('adoptant'), ('candidat'), ('famille_accueil'), ('autres');

INSERT INTO compatibilite(type) VALUES
('chat'), ('chien'), ('jeune enfant'), ('enfant'), ('jardin'), ('poney');
