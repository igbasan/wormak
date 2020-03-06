﻿using InterUserService.Data.Interfaces;
using InterUserService.Models.Implemetations;
using InterUserService.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterUserService.Data.Mongo
{
    public class TestimonialDAO : InterUserDAO<Testimonial>, ITestimonialDAO
    {
        public TestimonialDAO(IStoreDatabaseSettings settings) : base(settings)
        {
            _interUsers = database.GetCollection<Testimonial>(settings.TestimonialsCollectionName);
        }
    }
}
