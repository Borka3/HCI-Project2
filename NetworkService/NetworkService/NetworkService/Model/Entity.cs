using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class Entity : ValidationBase
    {
        private string textId;
        private string name;
        private int id;
        private double value;
        private EntityType typeEntity;

        public string TextId
        {
            get
            {
                return textId;
            }
            set
            {
                if(textId != value)
                {
                    textId = value;
                    OnPropertyChanged("TextId");
                }
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if(name != value)
                {
                    name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                if(id != value)
                {
                    id = value;
                    OnPropertyChanged("Id");
                   
                }
            }
        }

        public double Value
        {
            get
            {
                return this.value;
            }
            set
            {
                if(this.value != value)
                {
                    this.value = value;
                    OnPropertyChanged("Value");
                  
                }
            }
        }
        public EntityType TypeEntity
        {
            get
            {
                return typeEntity;
            }
            set
            {
                if(typeEntity != value)
                {
                    typeEntity = value;
                    TypeEntity.Name = value.Name;
                    TypeEntity.ImgSrc = value.ImgSrc;
                    OnPropertyChanged("TypeEntity");
                }
            }
        }

    

        public bool IsValidValue()
        {
            bool isValid = false;

            if(Value > 5 && Value < 16)
            {
                isValid = true;
            }
            return isValid;
        }
        protected override void ValidateSelf()
        {
            int tempId;
            bool parsingSuccess = int.TryParse(this.textId, out tempId);

            if(this.DoesIdAlreadyExist)
            {
                this.ValidationErrors["Id"] = "Id already exists.";
            }

            if(!parsingSuccess)
            {
                this.ValidationErrors["Id"] = "Id must be integer.";
            }
            else if(tempId < 0)
            {
                this.ValidationErrors["Id"] = "Id cannot be negative.";
            }

            if(string.IsNullOrWhiteSpace(this.textId))
            {
                this.ValidationErrors["Id"] = "Id is required.";
            }

            if(string.IsNullOrWhiteSpace(this.name))
            {
                this.ValidationErrors["Name"] = "Name is required.";
            }


        }

        public override string ToString()
        {
            return $"{Id} - {Name}";
        }


    }
}
