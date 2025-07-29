﻿using System.Windows.Documents;

namespace ViewByParameter.Models;

public interface IViewByParameterModel
{

    public List<ViewOption?> GetViewOption();
    public List<FilterOption?> GetFilterOption();
    public List<FilterFromProject?> GetFiltersFromProject();
    public List<string?> GetProjectParameters();

    public List<ElementsByParameter> GetElementsByParameter();
    public void Execute();

}